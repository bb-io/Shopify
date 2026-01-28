using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStorePage;
using Apps.Shopify.Models.Request.Page;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Online store pages")]
public class OnlineStorePageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search online store pages", Description = "Search online store pages with specific criteria")]
    public async Task<SearchPagesResponse> SearchPages([ActionParameter] SearchPagesRequest input)
    {
        var queryParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(input.TitleContains))
            queryParts.Add($"title:*{input.TitleContains}*");

        if (input.UpdatedAfter.HasValue)
            queryParts.Add($"updated_at:>{input.UpdatedAfter.Value:O}");

        if (input.UpdatedBefore.HasValue)
            queryParts.Add($"updated_at:<{input.UpdatedBefore.Value:O}");

        if (input.CreatedAfter.HasValue)
            queryParts.Add($"created_at:>{input.CreatedAfter.Value:O}");

        if (input.CreatedBefore.HasValue)
            queryParts.Add($"created_at:<{input.CreatedBefore.Value:O}");

        if (input.PublishedAfter.HasValue)
            queryParts.Add($"published_at:>{input.PublishedAfter.Value:O}");

        if (input.PublishedBefore.HasValue)
            queryParts.Add($"published_at:<{input.PublishedBefore.Value:O}");

        var variables = new Dictionary<string, object>();
        if (queryParts.Count != 0)
            variables["query"] = string.Join(" AND ", queryParts);

        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(
            GraphQlQueries.Pages,
            variables
        );

        return new(response);
    }

    [Action("Download online store page", Description = "Get content of a specific online store page")]
    public async Task<DownloadPageResponse> GetOnlineStorePageTranslationContent(
        [ActionParameter] OnlineStorePageRequest input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.PAGE);
        var request = new DownloadContentRequest
        {
            ContentId = input.OnlineStorePageId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload online store page", Description = "Update content of a specific online store page")]
    public async Task UpdateOnlineStorePageContent(
        [ActionParameter] UploadPageRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.PAGE);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = input.PageId,
            Locale = locale.Locale
        };

        await service.Upload(request);
    }
}