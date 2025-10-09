using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreArticle;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Online store articles")]
public class OnlineStoreArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : TranslatableResourceActions(invocationContext, fileManagementClient)
{
    [Action("List online store articles", Description = "List all aricles for the online store")]
    public async Task<ListArticlesResponse> ListArticles()
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = TranslatableResource.ONLINE_STORE_ARTICLE
        };
        var response = await Client
            .Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
                GraphQlQueries.TranslatableResources,
                variables);
        return new ListArticlesResponse
        {
            Articles = response.Select(x => new Article
            {
                ResourceId = x.ResourceId,
                Title = x.TranslatableContent.First(y => y.Key == "title").Value
            })
        };
    }

    [Action("Download online store article",
        Description = "Get content of a specific online store article")]
    public Task<FileResponse> GetOnlineStoreArticleTranslationContent(
        [ActionParameter] OnlineStoreArticleRequest input, [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetContentRequest getContentRequest)
        => GetResourceContent(input.OnlineStoreArticleId, locale.Locale, getContentRequest.Outdated ?? default, HtmlContentTypes.OnlineStoreArticle);

    [Action("Upload online store article",
        Description = "Upload content of a specific online store article")]
    public Task UpdateOnlineStoreArticleContent(
        [ActionParameter, DataSource(typeof(OnlineStoreArticleHandler)), Display("Online store article ID")]
        string? onlineStoreArticleId,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(onlineStoreArticleId, locale.Locale, file.File);
}