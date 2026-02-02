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

namespace Apps.Shopify.Services;

public class TranslatableResourceService(InvocationContext invocationContext,
    IFileManagementClient fileManagementClient) : ShopifyInvocable(invocationContext)
{
    private const int MaxUpdateChunkSize = 250;

    public async Task<FileReference> GetResourceContent(string resourceId, string locale, bool outdated, string contentType)
    {
        var translatableContent = await GetTranslatableContent(resourceId, locale, outdated);
        var html = ShopifyHtmlConverter.ToHtml(translatableContent, contentType);

        return await fileManagementClient.UploadAsync(
            html,
            MediaTypeNames.Text.Html,
            $"{resourceId.GetShopifyItemId()}.html"
        );
    }

    public async Task<List<IdentifiedContentEntity>> GetTranslatableContent(string resourceId, string locale, bool outdated)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId,
                locale,
                outdated
            }
        };
        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        return response.TranslatableResource.GetTranslatableContent()
            .Select(x => new IdentifiedContentEntity(x)
            {
                Id = resourceId
            }).ToList();
    }

    public async Task UpdateResourceContent(string? resourceId, string locale, FileReference file)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, file);
        var items = ShopifyHtmlConverter.ToJson(html, locale).ToList();

        await UpdateIdentifiedContent(items, resourceId);
    }

    public async Task UpdateIdentifiedContent(ICollection<IdentifiedContentRequest>? items, string? resourceId = null)
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
            var id = group.Key;
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new PluginMisconfigurationException(
                    $"Content ID is missing. Please it in the input or include it in the file"
                );
            }

            var groupItems = group.ToList();

            if (groupItems.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
            {
                var sourceContent = await GetResourceSourceContent(id);
                groupItems.ForEach(x =>
                    x.TranslatableContentDigest = sourceContent.TranslatableResource.TranslatableContent
                        .FirstOrDefault(y => y.Key == x.Key)?.Digest ?? string.Empty);
            }

            var validItems = groupItems
                .Where(x => !string.IsNullOrWhiteSpace(x.TranslatableContentDigest))
                .Select(x => new TranslatableResourceContentRequest(x))
                .Chunk(MaxUpdateChunkSize);

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
        TranslatableResource resourceType, string? locale = default, bool outdated = false)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = resourceType,
            ["locale"] = locale ?? string.Empty,
            ["outdated"] = outdated
        };
        return await Client
            .Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
                GraphQlQueries.TranslatableResourcesWithTranslations,
                variables, default);
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
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceContent,
            Variables = new
            {
                resourceId
            }
        };
        return Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
    }
}
