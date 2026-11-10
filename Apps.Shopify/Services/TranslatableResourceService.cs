using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;
using System.Net.Mime;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Models.Response.Utility.TranslationError;

namespace Apps.Shopify.Services;

public class TranslatableResourceService(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    // Max 100 translation keys per mutation - do not change
    private const int MaxUpdateChunkSize = 100;

    public async Task<FileRecord> GetResourceContent(string resourceId, string locale, bool outdated, ShopifyMetadata metadata)
    {
        var resource = await GetTranslatableResource(resourceId, locale, outdated, metadata.MarketId);
        var html = ShopifyHtmlConverter.ToHtml(ToIdentifiedContent(resource, resourceId), metadata);

        return new FileRecord(html, MediaTypeNames.Text.Html, resourceId.GetFileName(metadata.MarketId, resource.GetName()));
    }

    public async Task<List<IdentifiedContentEntity>> GetTranslatableContent(
        string resourceId,
        string locale,
        bool outdated,
        string? marketId)
    {
        var resource = await GetTranslatableResource(resourceId, locale, outdated, marketId);
        return ToIdentifiedContent(resource, resourceId);
    }

    public async Task UpdateResourceContent(string? resourceId, string locale, string htmlContent, string? marketId = null)
    {
        var items = ShopifyHtmlConverter
            .ToJson(htmlContent, locale, new ShopifyMetadata { MarketId = marketId })
            .ToList();

        await UpdateIdentifiedContent(items, resourceId, marketId);
    }

    public async Task UpdateIdentifiedContent(
        ICollection<IdentifiedContentRequest>? items,
        string? resourceId = null,
        string? marketId = null)
    {
        if (items is null || items.Count == 0)
            return;

        if (!string.IsNullOrWhiteSpace(resourceId))
        {
            foreach (var item in items)
            {
                if (item.ResourceId != resourceId)
                    item.TranslatableContentDigest = string.Empty;

                item.ResourceId = resourceId;
            }
        }

        if (!string.IsNullOrWhiteSpace(marketId))
        {
            foreach (var item in items)
                item.MarketId = marketId;
        }

        var groupedItems = items.GroupBy(x => x.ResourceId).ToArray();

        foreach (var group in groupedItems)
        {
            string id = group.Key;
            if (string.IsNullOrWhiteSpace(id))
                throw new PluginMisconfigurationException("Content ID is missing. Please provide it in the input");

            var groupItems = group.ToList();

            var sourceContent = await GetResourceSourceContent(id);
            if (sourceContent.TranslatableResource is null)
            {

                if (id.StartsWith("gid://shopify/Metaobject/", StringComparison.Ordinal))
                {
                    InvocationContext.Logger?.LogWarning(
                        $"[Shopify] Skipping metaobject '{id}'. Shopify returned no " +
                        "translatable resource. The object may be missing, inaccessible, " +
                        "or its definition may not allow translations.",
                        Array.Empty<object>());
                    continue;
                }

                throw new PluginMisconfigurationException(
                    $"Could not find source content for resource {id}. Please check the input");
            }

            var sourceByKey = sourceContent.TranslatableResource.TranslatableContent
                .GroupBy(x => x.Key)
                .ToDictionary(g => g.Key, g => g.First());


            var validItems = new List<TranslatableResourceContentRequest>();
            var skippedCount = 0;

            foreach (var item in groupItems)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    InvocationContext.Logger?.LogWarning(
                        $"[Shopify] Skipping blank translation. Resource: '{id}'; " +
                        $"key: '{item.Key}'.",
                        Array.Empty<object>());
                    skippedCount++;
                    continue;
                }

                // Theme and other translation keys may no longer exist in Shopify.
                if (!sourceByKey.TryGetValue(item.Key, out var source))
                {
                    InvocationContext.Logger?.LogWarning(
                        $"[Shopify] Skipping translation. Resource: '{id}'; " +
                        $"key: '{item.Key}' is absent from Shopify's current " +
                        "translatable content.",
                        Array.Empty<object>());
                    skippedCount++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(source.Digest))
                {
                    InvocationContext.Logger?.LogWarning(
                        $"[Shopify] Skipping translation. Resource: '{id}'; " +
                        $"key: '{item.Key}'. Shopify did not provide a source digest.",
                        Array.Empty<object>());
                    skippedCount++;
                    continue;
                }

                if (!string.Equals(item.TranslatableContentDigest, source.Digest,
                        StringComparison.Ordinal))
                {
                    InvocationContext.Logger?.LogWarning(
                        $"[Shopify] Digest mismatch. Resource: '{id}'; " +
                        $"key: '{item.Key}'; file digest: " +
                        $"'{item.TranslatableContentDigest}'; Shopify digest: " +
                        $"'{source.Digest}'. Using current Shopify digest.",
                        Array.Empty<object>());
                }

                item.TranslatableContentDigest = source.Digest;

                if (string.Equals(item.Value.Trim(), source.Value?.Trim(),
                        StringComparison.Ordinal))
                    continue;

                validItems.Add(new TranslatableResourceContentRequest(item));
            }

            if (validItems.Count == 0)
            {
                if (skippedCount > 0)
                {
                    InvocationContext.Logger?.LogWarning(
                        $"[Shopify] No translations uploaded for '{id}': " +
                        $"{skippedCount} invalid or unsupported field(s) skipped.",
                        Array.Empty<object>());
                }
                continue;
            }

            foreach (var chunk in validItems.Chunk(MaxUpdateChunkSize))
            {
                var request = new GraphQLRequest
                {
                    Query = GraphQlMutations.TranslationsRegister,
                    Variables = new
                    {
                        resourceId = id,
                        translations = chunk
                    }
                };
                var response = await Client.ExecuteWithErrorHandling<TranslationsRegisterResponse>(request);

                var errors = response.TranslationsRegister.UserErrors;
                if (errors.Count > 0)
                    throw new PluginApplicationException($"Shopify rejected translations for {id}: {string.Join("; ", errors)}");
            }
        }
    }

    public async Task<ICollection<TranslatableResourceEntity>> ListTranslatableResources(
        TranslatableResource resourceType,
        string? locale = null,
        bool outdated = false,
        string? marketId = null)
    {
        var variables = new Dictionary<string, object>
        {
            ["resourceType"] = resourceType,
            ["locale"] = locale ?? string.Empty,
            ["outdated"] = outdated
        };

        if (!string.IsNullOrEmpty(marketId))
            variables["marketId"] = marketId;

        return await Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResourcesWithTranslations,
            variables);
    }

    public async Task<ICollection<IdentifiedContentEntity>> ListIdentifiedTranslatableResources(
        TranslatableResource resourceType, string? locale = default, bool outdated = false)
    {
        var entities = await ListTranslatableResources(resourceType, locale, outdated);
        return entities
            .SelectMany(x => x.GetTranslatableContent().Select(y => new IdentifiedContentEntity(y)
            {
                Id = x.ResourceId
            }))
            .ToList();
    }

    public Task<TranslatableResourceResponse> GetResourceSourceContent(string resourceId)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceContent,
            Variables = new { resourceId }
        };
        return Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
    }

    public async Task<IEnumerable<IdentifiedContentEntity>> GetProductMetafields(
        string productId,
        string locale,
        bool outdated = false,
        string? market = null,
        HashSet<string>? keys = null,
        string? nameContains = null)
    {
        var productMetafields = await Client.Paginate<MetafieldEntity, ProductMetafieldsPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            new Dictionary<string, object> { ["resourceId"] = productId });

        var metafieldIds = productMetafields
            .Where(x => keys is null || keys.Contains(x.ToString()))
            .Where(x => string.IsNullOrEmpty(nameContains) ||
                        x.Definition?.Name?.Contains(nameContains, StringComparison.OrdinalIgnoreCase) == true)
            .Select(x => x.Id)
            .ToArray();

        if (metafieldIds.Length == 0)
            return [];

        var variables = new Dictionary<string, object>
        {
            ["resourceIds"] = metafieldIds,
            ["locale"] = locale,
            ["outdated"] = outdated
        };

        if (!string.IsNullOrEmpty(market))
            variables["marketId"] = market;

        var resources = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcesByIdsPaginationResponse>(
            GraphQlQueries.TranslatableResourcesByIds,
            variables);

        return resources.SelectMany(x => x.GetTranslatableContent().Select(y => new IdentifiedContentEntity(y) { Id = x.ResourceId }));
    }

    private async Task<TranslatableResourceEntity> GetTranslatableResource(
        string resourceId,
        string locale,
        bool outdated,
        string? marketId)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new { resourceId, locale, outdated, marketId }
        };

        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        return response.TranslatableResource ??
               throw new PluginMisconfigurationException($"Could not find translatable content for {resourceId}. Please check the input");
    }

    private static List<IdentifiedContentEntity> ToIdentifiedContent(TranslatableResourceEntity resource, string resourceId)
    {
        return resource.GetTranslatableContent()
            .Select(x => new IdentifiedContentEntity(x) { Id = resourceId })
            .ToList();
    }
}
