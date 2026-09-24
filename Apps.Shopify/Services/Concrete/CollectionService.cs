using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Collection;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Collection;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class CollectionService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService, IPollingContentService
{
    public override string ContentType => TranslatableResources.Collection;

    public async Task<FileRecord> Download(DownloadContentRequest input)
    {
        var metadata = new ShopifyMetadata
        {
            MarketId = input.MarketId,
            ContentType = ContentType.ToLower()
        };
        
        return await ResourceService.GetResourceContent(input.ContentId, input.Locale, input.Outdated ?? false, metadata);
    }

    public async Task<ContentUpdatedResponse> PollUpdated(DateTime after, DateTime before, PollUpdatedContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", after, before)
            .Build();

        var response = await Client.Paginate<CollectionEntity, CollectionsPaginationResponse>(
            GraphQlQueries.Collections,
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
            .Build();

        var response = await Client.Paginate<CollectionEntity, CollectionsPaginationResponse>(
            GraphQlQueries.Collections,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
        return new(items);
    }
}
