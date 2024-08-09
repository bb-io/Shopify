using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStorePage;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStorePageActions : TranslatableResourceActions
{
    public OnlineStorePageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("List online store pages", Description = "List all pages in the online store")]
    public async Task<ListPagesResponse> ListPages()
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = TranslatableResource.ONLINE_STORE_PAGE
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

    [Action("Get online store page content as HTML",
        Description = "Get content of a specific online store page in HTML format")]
    public Task<FileResponse> GetOnlineStorePageTranslationContent(
        [ActionParameter] OnlineStorePageRequest input, [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetContentRequest getContentRequest)
        => GetResourceContent(input.OnlineStorePageId, locale.Locale, getContentRequest.Outdated ?? default, HtmlContentTypes.OnlineStorePageContent);

    [Action("Update online store page content from HTML",
        Description = "Update content of a specific online store page from HTML file")]
    public Task UpdateOnlineStorePageContent(
        [ActionParameter, DataSource(typeof(OnlineStorePageHandler)), Display("Online store page ID")]
        string? onlineStorePageId,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(onlineStorePageId, locale.Locale, file.File);
}