using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class ArticleService(InvocationContext invocationContext) 
    : BaseContentService(invocationContext), IContentService, IPollingContentService
{
    protected override string ContentType => TranslatableResources.Article;
    
    public Task<FileRecord> Download(DownloadContentRequest input)
    {
        return DownloadTranslatableResource(input);
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

        // API 'title' filter does not work
        if (!string.IsNullOrEmpty(input.NameContains))
        {
            response = response.Where(x =>
                x.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        var items = response.Select(x => 
            new PollingContentItemEntity(x.Id, ContentType, x.Title, x.UpdatedAt ?? x.CreatedAt)
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

        var items = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
        return new(items);
    }
}
