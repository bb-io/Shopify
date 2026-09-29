using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Article;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStoreArticle;
using Apps.Shopify.Models.Response.Article;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Articles")]
public class OnlineStoreArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.Article;

    [Action("Search articles", Description = "Search articles with specific criteria")]
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

    [Action("Download article", Description = "Download content of a specific article")]
    public async Task<DownloadArticleResponse> GetOnlineStoreArticleTranslationContent(
        [ActionParameter] ArticleIdentifier input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = input.ArticleId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }

    [Action("Upload article", Description = "Upload content of a specific article")]
    public Task UpdateOnlineStoreArticleContent(
        [ActionParameter] UploadArticleRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ArticleId, locale.Locale, marketIdentifier.MarketId);
    }
}