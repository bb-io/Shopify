using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Entities.Product;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Models.Response.Product;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;
using System.Net.Mime;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Services.Base;
using Apps.Shopify.Services.Models;

namespace Apps.Shopify.Services.Concrete;

public class ProductService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService, IPollingContentService
{
    protected override string ContentType => TranslatableResources.Product;

    public async Task<FileRecord> Download(DownloadContentRequest input)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = input.ContentId,
                locale = input.Locale,
                outdated = input.Outdated ?? false,
                marketId = input.MarketId
            }
        };
        
        var productContent = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        if (productContent.TranslatableResource is null)
            throw new PluginMisconfigurationException(
                $"Could not find translatable content for product {input.ContentId}. Please check the input");

        var productInfo = 
            input.IncludeOptions is true || input.IncludeOptionValues is true
            ? await GetProductInfo(input.ContentId, input.Locale, input.MarketId)
            : new();

        var productContentEntities = productContent.TranslatableResource.GetTranslatableContent()
            .Select(x => new IdentifiedContentEntity(x) { Id = input.ContentId });
        
        var metafieldKeys = input.IncludeMetafields is true ? null : input.MetafieldKeys?.ToHashSet();
        var metafieldContentEntities = input.IncludeMetafields is true || metafieldKeys?.Count > 0
            ? await GetProductMetafields(input.ContentId, input.Locale, input.Outdated ?? false, input.MarketId, metafieldKeys)
            : [];

        var optionsContentEntities = input.IncludeOptions is true ? GetProductOptions(productInfo) : [];
        var optionValuesContentEntities = input.IncludeOptionValues is true ? GetProductOptionValues(productInfo) : [];
        
        var html = ShopifyHtmlConverter.ProductToHtml(new()
        {
            MarketId = input.MarketId,
            ProductContentEntities = productContentEntities,
            MetafieldsContentEntities = metafieldContentEntities,
            OptionsContentEntities = optionsContentEntities,
            OptionValuesContentEntities = optionValuesContentEntities,
        });

        return new FileRecord(html, MediaTypeNames.Text.Html, input.ContentId.GetFileName(input.MarketId));
    }

    public async Task<ContentUpdatedResponse> PollUpdated(DateTime after, DateTime before, PollUpdatedContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", after, before)
            .Build();

        var response = await Client.Paginate<ProductEntity, ProductsPaginationResponse>(
            GraphQlQueries.Products,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new PollingContentItemEntity(x.Id, ContentType, x.Title, x.UpdatedAt)).ToList();
        return new(items);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input, bool firstPageOnly = false, CancellationToken token = default)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .Build();

        var response = await Client.Paginate<ProductEntity, ProductsPaginationResponse>(
            GraphQlQueries.Products,
            QueryHelper.QueryToDictionary(query),
            firstPageOnly,
            token);

        var items = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
        return new(items);
    }

    public Task Upload(UploadContentServiceRequest input)
    {
        var dto = ShopifyHtmlConverter.ProductToJson(input.HtmlContent, input.Locale, input.MarketId);

        var allItems = new List<IdentifiedContentRequest>();
        var productItems = dto.ProductContentEntities.ToList();
        if (!string.IsNullOrWhiteSpace(input.ContentId))
            foreach (var item in productItems) item.ResourceId = input.ContentId;

        allItems.AddRange(productItems);

        if (dto.MetafieldsContentEntities != null) 
            allItems.AddRange(dto.MetafieldsContentEntities);
        if (dto.OptionsContentEntities != null) 
            allItems.AddRange(dto.OptionsContentEntities);
        if (dto.OptionValuesContentEntities != null) 
            allItems.AddRange(dto.OptionValuesContentEntities);

        return ResourceService.UpdateIdentifiedContent(allItems, null, input.MarketId);
    }

    private static IEnumerable<IdentifiedContentEntity> GetProductOptions(ProductEntity product)
    {
        var productOptions = product.Options ?? [];
        var options = productOptions.SelectMany(x => x.Translations.Select(y => new IdentifiedContentEntity(y)
        {
            Id = x.Id
        })).ToArray();

        return options.Any()
            ? options
            : productOptions.Select(x => new IdentifiedContentEntity()
            {
                Id = x.Id,
                Key = "name",
                Value = x.Name
            });
    }

    private static IEnumerable<IdentifiedContentEntity>? GetProductOptionValues(ProductEntity product)
    {
        var productOptions = product.Options ?? [];
        var values = productOptions.SelectMany(x => x.OptionValues.SelectMany(y => y.Translations.Select(x =>
            new IdentifiedContentEntity(x)
            {
                Id = y.Id
            }))).ToArray();

        return values.Any()
            ? values
            : productOptions.SelectMany(x => x.OptionValues.Select(x => new IdentifiedContentEntity()
            {
                Id = x.Id,
                Key = "name",
                Value = x.Name
            }));
    }

    private async Task<ProductEntity> GetProductInfo(string productId, string locale, string? marketId)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.Product,
            Variables = new
            {
                resourceId = productId,
                locale,
                marketId
            }
        };

        var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);
        return response.Product ?? throw new PluginMisconfigurationException(
            $"Could not find product {productId}. Please check the input");
    }

    private async Task<IEnumerable<IdentifiedContentEntity>?> GetProductMetafields(
        string productId,
        string locale,
        bool outdated = false,
        string? market = null,
        HashSet<string>? keys = null)
    {
        var productMetafields = await Client.Paginate<MetafieldEntity, ProductMetafieldsPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            new Dictionary<string, object> { ["resourceId"] = productId });
        
        if (productMetafields.Count == 0)
            return [];
        
        var metafieldIds = productMetafields
            .Where(x => keys is null || keys.Contains(x.ToString()))
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
}
