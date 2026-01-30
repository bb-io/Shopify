using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Services.Concrete;

public class ArticleService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService, IPollingContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        return await _resourceService.GetResourceContent(
            input.ContentId, 
            input.Locale, 
            input.Outdated ?? default, 
            HtmlMetadataConstants.OnlineStoreArticle
        );
    }

    public async Task<ContentUpdatedResponse> PollUpdated(DateTime after, DateTime before, PollUpdatedContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddDateRange("updated_at", after, before)
            .Build();

        var response = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            QueryHelper.QueryToDictionary(query)
        );

        if (!string.IsNullOrEmpty(input.NameContains))
        {
            response = response.Where(x =>
                x.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        var items = response.Select(x => 
            new PollingContentItemEntity(x.Id, "Article", x.Title, x.UpdatedAt ?? x.CreatedAt)
        ).ToList();
        return new(items);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .Build();

        var response = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            QueryHelper.QueryToDictionary(query)
        );

        if (!string.IsNullOrEmpty(input.NameContains))
        {
            response = response.Where(x =>
                x.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        var items = response.Select(x => new ContentItemEntity(x.Id, "Article", x.Title)).ToList();
        return new(items);
    }

    public async Task Upload(UploadContentRequest input)
    {
        await _resourceService.UpdateResourceContent(input.ContentId, input.Locale, input.Content);
    }
}
