using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using System.Net.Mime;
using Apps.Shopify.HtmlConversion.Models;

namespace Apps.Shopify.Services;

public class TranslatableResourceService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    public async Task<FileReference> GetResourceContent(string resourceId, string locale, bool outdated, ShopifyMetadata metadata)
    {
        var translatableContent = await GetTranslatableContent(resourceId, locale, outdated, metadata.MarketId);
        var html = ShopifyHtmlConverter.ToHtml(translatableContent, metadata);

        return await fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, resourceId.GetFileName(metadata.MarketId));
    }

    public async Task<List<IdentifiedContentEntity>> GetTranslatableContent(
        string resourceId, 
        string locale, 
        bool outdated,
        string? marketId)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId,
                locale,
                outdated,
                marketId
            }
        };
        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        return response.TranslatableResource.GetTranslatableContent()
            .Select(x => new IdentifiedContentEntity(x)
            {
                Id = resourceId
            }).ToList();
    }

    public async Task UpdateResourceContent(string? resourceId, string locale, FileReference file, string? marketId = null)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, file);
        var items = ShopifyHtmlConverter
            .ToJson(html, locale, new ShopifyMetadata { MarketId = marketId })
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
                item.ResourceId = resourceId;
        }

        var groupedItems = items.GroupBy(x => x.ResourceId).ToArray();

        foreach (var group in groupedItems)
        {
            string id = group.Key;
            if (string.IsNullOrWhiteSpace(id))
                throw new PluginMisconfigurationException("Content ID is missing. Please provide it in the input");

            var groupItems = group.ToList();

            var sourceContent = await GetResourceSourceContent(id);
            var sourceByKey = sourceContent.TranslatableResource.TranslatableContent
                .GroupBy(x => x.Key)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var item in groupItems)
            {
                if (string.IsNullOrWhiteSpace(item.TranslatableContentDigest))
                    item.TranslatableContentDigest = sourceByKey.GetValueOrDefault(item.Key)?.Digest ?? string.Empty;
            }

            var withDigest = groupItems
                .Where(x => !string.IsNullOrWhiteSpace(x.TranslatableContentDigest))
                .ToArray();

            if (withDigest.Length == 0)
                throw new PluginApplicationException($"Could not resolve content digests for {id}. Nothing was uploaded");

            var validItems = withDigest
                .Where(x => x.Value?.Trim() != sourceByKey.GetValueOrDefault(x.Key)?.Value?.Trim())
                .Select(x => new TranslatableResourceContentRequest(x))
                .ToArray();

            if (validItems.Length == 0)
                continue;

            foreach (var chunk in validItems)
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
                await Client.ExecuteWithErrorHandling(request);
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
            ["outdated"] = outdated,
            ["marketId"] = marketId ?? string.Empty
        };
        
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
}
