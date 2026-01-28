using Apps.Shopify.Constants.GraphQL;
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

[ActionList("Online store articles")]
public class OnlineStoreArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search online store articles", Description = "Search aricles of the online store with specific criteria")]
    public async Task<SearchArticlesResponse> SearchArticles([ActionParameter] SearchArticlesRequest input)
    {
        var queryParts = new List<string>();

        if (input.PublishedBefore.HasValue)
            queryParts.Add($"published_at:<{input.PublishedBefore.Value:O}");
        if (input.PublishedAfter.HasValue)
            queryParts.Add($"published_at:>{input.PublishedAfter.Value:O}");

        if (input.CreatedBefore.HasValue)
            queryParts.Add($"created_at:<{input.CreatedBefore.Value:O}");
        if (input.CreatedAfter.HasValue)
            queryParts.Add($"created_at:>{input.CreatedAfter.Value:O}");

        if (input.UpdatedBefore.HasValue)
            queryParts.Add($"updated_at:<{input.UpdatedBefore.Value:O}");
        if (input.UpdatedAfter.HasValue)
            queryParts.Add($"updated_at:>{input.UpdatedAfter.Value:O}");

        var variables = new Dictionary<string, object>();
        if (queryParts.Count != 0)
            variables["query"] = string.Join(" AND ", queryParts);

        var response = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            variables
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

    [Action("Download online store article", Description = "Get content of a specific online store article")]
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

    [Action("Upload online store article", Description = "Upload content of a specific online store article")]
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