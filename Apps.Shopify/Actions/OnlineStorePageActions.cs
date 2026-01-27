using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStorePage;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Models.Response.TranslatableResource;
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

    [Action("List online store pages", Description = "List all pages in the online store")]
    public async Task<ListPagesResponse> ListPages()
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = TranslatableResource.PAGE
        };
        var response = await Client
            .Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
                GraphQlQueries.TranslatableResources,
                variables);
        return new ListPagesResponse
        {
            Pages = response.Select(x => new Page
            {
                ResourceId = x.ResourceId,
                Title = x.TranslatableContent.First(y => y.Key == "title").Value
            })
        };
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