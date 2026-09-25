using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStorePage;
using Apps.Shopify.Models.Request.Page;
using Apps.Shopify.Models.Response.Page;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Pages")]
public class OnlineStorePageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.Page;

    [Action("Search pages", Description = "Search pages with specific criteria")]
    public async Task<SearchPagesResponse> SearchPages([ActionParameter] SearchPagesRequest input)
    {
        input.ValidateDates();

        string? query = new QueryBuilder()
            .AddContains("title", input.TitleContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .Build();

        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(
            GraphQlQueries.Pages,
            QueryHelper.QueryToDictionary(query)
        );

        return new(response);
    }

    [Action("Download page", Description = "Download content of a specific page")]
    public async Task<DownloadPageResponse> GetOnlineStorePageTranslationContent(
        [ActionParameter] PageIdentifier input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = input.PageId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }

    [Action("Upload page", Description = "Upload content of a specific page")]
    public Task UpdateOnlineStorePageContent(
        [ActionParameter] UploadPageRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.PageId, locale.Locale, marketIdentifier.MarketId);
    }
}