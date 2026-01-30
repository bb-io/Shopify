using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Article;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStoreArticle;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Articles")]
public class OnlineStoreArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search articles", Description = "Search aricles with specific criteria")]
    public async Task<SearchArticlesResponse> SearchArticles([ActionParameter] SearchArticlesRequest input)
    {
        input.ValidateDates();

        string? query = new QueryBuilder()
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .Build();

        var response = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            QueryHelper.QueryToDictionary(query)
        );

        if (!string.IsNullOrEmpty(input.TitleContains))
        {
            response = response.Where(x =>
                x.Title.Contains(input.TitleContains, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        var result = response.Select(x => new GetArticleResponse(x));
        return new(result);
    }

    [Action("Download article", Description = "Get content of a specific online store article")]
    public async Task<DownloadArticleResponse> GetOnlineStoreArticleTranslationContent(
        [ActionParameter] OnlineStoreArticleRequest input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.ARTICLE);
        var request = new DownloadContentRequest
        {
            ContentId = input.OnlineStoreArticleId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload article", Description = "Upload content of a specific article")]
    public async Task UpdateOnlineStoreArticleContent(
        [ActionParameter] UploadArticleRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.ARTICLE);
        var request = new UploadContentRequest
        {
            ContentId = input.ArticleId,
            Content = input.File,
            Locale = locale.Locale,
        };

        await service.Upload(request);
    }
}