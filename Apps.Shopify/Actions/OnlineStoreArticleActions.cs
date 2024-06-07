using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreArticle;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStoreArticleActions : TranslatableResourceActions
{
    public OnlineStoreArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

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

    [Action("Get online store article translation as HTML",
        Description = "Get content of a specific online store article in HTML format")]
    public Task<FileResponse> GetOnlineStoreArticleTranslationContent(
        [ActionParameter] OnlineStoreArticleRequest input, [ActionParameter] LocaleRequest locale)
        => GetResourceContent(input.OnlineStoreArticleId, locale.Locale);

    [Action("Update online store article content from HTML",
        Description = "Update content of a specific online store article from HTML file")]
    public Task UpdateOnlineStoreArticleContent([ActionParameter] OnlineStoreArticleRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.OnlineStoreArticleId, locale.Locale, file.File);
}