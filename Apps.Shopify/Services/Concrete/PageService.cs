using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Page;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class PageService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService, IPollingContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext);
    public override string ContentType => TranslatableResources.Page;

    public Task<FileRecord> Download(DownloadContentRequest input)
    {
        var metadata = new ShopifyMetadata
        {
            MarketId = input.MarketId,
            ContentType = ContentType.ToLower()
        };
        
        return _resourceService.GetResourceContent(input.ContentId, input.Locale, input.Outdated ?? false, metadata);
    }

    public async Task<ContentUpdatedResponse> PollUpdated(DateTime after, DateTime before, PollUpdatedContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", after, before)
            .Build();

        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(
            GraphQlQueries.Pages,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new PollingContentItemEntity(x.Id, ContentType, x.Title, x.UpdatedAt)).ToList();
        return new(items);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .Build();

        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(
            GraphQlQueries.Pages,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
        return new(items);
    }
}
