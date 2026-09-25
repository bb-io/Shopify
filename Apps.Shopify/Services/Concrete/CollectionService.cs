using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Collection;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Collection;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class CollectionService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService, IPollingContentService
{
    protected override string ContentType => TranslatableResources.Collection;

    public Task<FileRecord> Download(DownloadContentRequest input)
    {
        return DownloadTranslatableResource(input);
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

    public Task Upload(UploadContentServiceRequest input)
    {
        return UploadTranslatableResource(input);
    }
}
