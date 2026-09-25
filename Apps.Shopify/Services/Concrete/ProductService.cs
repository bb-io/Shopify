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
        
        var metafieldContentEntities = input.IncludeMetafields is true
            ? await GetProductMetafields(input.ContentId, input.Locale, input.Outdated ?? false, input.MarketId)
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

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .Build();

        var response = await Client.Paginate<ProductEntity, ProductsPaginationResponse>(
            GraphQlQueries.Products,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
        return new(items);
    }

    public override async Task Upload(UploadContentServiceRequest input)
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

        await ResourceService.UpdateIdentifiedContent(allItems, null, input.MarketId);
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
        string? market = null)
    {
        var productMetaFields = await Client.Paginate<MetafieldEntity, ProductMetafieldsPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            new Dictionary<string, object> { ["resourceId"] = productId }
        );
        
        var metaFields = await ResourceService.ListTranslatableResources(
            TranslatableResource.METAFIELD, 
            locale, 
            outdated,
            market);

        var resources = metaFields.Where(x => productMetaFields.Any(y => x.ResourceId == y.Id)).ToArray();
        var content = resources.All(x => !x.Translations.Any())
            ? resources.Select(x => (x.ResourceId, x.TranslatableContent.FirstOrDefault())).ToArray()
            : resources.Select(x => (x.ResourceId, x.Translations.FirstOrDefault())).ToArray();

        return content.Select(x => new IdentifiedContentEntity(x.Item2) { Id = x.ResourceId });
    }
}
