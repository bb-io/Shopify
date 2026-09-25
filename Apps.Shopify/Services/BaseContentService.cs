using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Polling;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.TranslatableResource;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services;

public abstract class BaseContentService(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    protected abstract string ContentType { get; }
    
    protected readonly TranslatableResourceService ResourceService = new(invocationContext);

    protected Task UploadTranslatableResource(UploadContentServiceRequest input)
    {
        return ResourceService.UpdateResourceContent(input.ContentId, input.Locale, input.HtmlContent, input.MarketId);
    }
    
    protected async Task<SearchContentResponse> SearchTranslatableResources(SearchContentRequest input)
    {
        var translatableResources = await ListTranslatableResources();

        var items = translatableResources
            .Where(x => x.MatchesSearch(input.NameContains))
            .Select(x => new ContentItemEntity(x.ResourceId, ContentType, x.GetDisplayName()))
            .ToList();

        return new(items);
    }
    
    protected Task<FileRecord> DownloadTranslatableResource(DownloadContentRequest input)
    {
        var metadata = new ShopifyMetadata
        {
            MarketId = input.MarketId,
            ContentType = ContentType.ToLower()
        };

        return ResourceService.GetResourceContent(input.ContentId, input.Locale, input.Outdated ?? false, metadata);
    }
    
    protected async Task<DigestPollResult> PollTranslatableResourceDigests(
        IReadOnlyDictionary<string, string> knownDigests,
        PollUpdatedContentRequest input)
    {
        var translatableResources = await ListTranslatableResources();

        var items = translatableResources
            .Where(x => x.MatchesSearch(input.NameContains))
            .Select(x => new DigestItemEntity(x.ResourceId, x.GetDisplayName(), x.GetContentDigest()));

        return BuildDigestPollResult(knownDigests, items);
    }

    protected DigestPollResult BuildDigestPollResult(
        IReadOnlyDictionary<string, string> knownDigests,
        IEnumerable<DigestItemEntity> items)
    {
        var itemList = items.ToList();

        var current = itemList.ToDictionary(x => x.Id, x => x.Digest);
        var changed = itemList
            .Where(x => !knownDigests.TryGetValue(x.Id, out var known) || known != x.Digest)
            .Select(x => new PollingContentItemEntity(x.Id, ContentType, x.Name))
            .ToList();

        return new(changed, current);
    }
    
    protected Task<List<TranslatableResourceEntity>> ListTranslatableResources(TranslatableResource? resourceType = null)
    {
        var variables = new Dictionary<string, object>
        {
            ["resourceType"] = resourceType ?? TranslatableResources.GetApiType(ContentType)
        };

        return Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            variables);
    }
}