using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Response.TranslatableResource;

namespace Apps.Shopify.Services.Concrete;

public class MetafieldService(InvocationContext invocationContext) : BaseContentService(invocationContext), IContentService
{
    public override string ContentType => TranslatableResources.Metafield;

    public async Task<FileRecord> Download(DownloadContentRequest input)
    {
        var metadata = new ShopifyMetadata
        {
            MarketId = input.MarketId,
            ContentType = ContentType.ToLower()
        };
        
        return await ResourceService.GetResourceContent(input.ContentId, input.Locale, input.Outdated ?? false, metadata);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        var variables = new Dictionary<string, object>
        {
            ["resourceType"] = TranslatableResources.GetApiType(ContentType)
        };

        var response = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            variables
        );

        var items = response
            .Select(x => new ContentItemEntity(x.ResourceId, ContentType, x.ToString()))
            .Where(x => string.IsNullOrEmpty(input.NameContains) ||
                        x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase) ||
                        x.ContentId.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        return new(items);
    }
}
