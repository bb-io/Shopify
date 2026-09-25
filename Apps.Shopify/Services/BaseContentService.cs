using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Content;
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
        var variables = new Dictionary<string, object>
        {
            ["resourceType"] = TranslatableResources.GetApiType(ContentType)
        };

        var response = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            variables);

        var items = response
            .Select(x => new ContentItemEntity(x.ResourceId, ContentType, x.GetDisplayName()))
            .Where(x => string.IsNullOrEmpty(input.NameContains) ||
                        x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase) ||
                        x.ContentId.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase))
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
}