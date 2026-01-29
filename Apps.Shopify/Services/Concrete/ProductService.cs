using Apps.Shopify.Api;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Product;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Models.Response.Product;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using System.Net.Mime;

namespace Apps.Shopify.Services.Concrete;

public class ProductService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = input.ContentId,
                locale = input.Locale,
                outdated = input.Outdated ?? false
            }
        };
        var productContent = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);

        var productInfo = 
            input.IncludeOptions is true || input.IncludeOptionValues is true
            ? await GetProductInfo(input.ContentId, input.Locale)
            : new();

        var html = ShopifyHtmlConverter.ProductToHtml(new()
        {
            ProductContentEntities = productContent.TranslatableResource.GetTranslatableContent()
                .Select(x => new IdentifiedContentEntity(x) { Id = input.ContentId }),
            MetafieldsContentEntities = input.IncludeMetafields is true
                ? await GetProductMetafields(input.ContentId, input.Locale, input.Outdated ?? default)
                : [],
            OptionsContentEntities = input.IncludeOptions is true ? GetProductOptions(productInfo) : [],
            OptionValuesContentEntities = input.IncludeOptionValues is true ? GetProductOptionValues(productInfo) : [],
        });

        return await fileManagementClient.UploadAsync(
            html, 
            MediaTypeNames.Text.Html,
            $"{input.ContentId.GetShopifyItemId()}.html"
        );
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
            GraphQlQueries.Product,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new ContentItemEntity(x.Id, "Product", x.Title)).ToList();
        return new(items);
    }

    public async Task Upload(UploadContentRequest input)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, input.Content);
        var dto = ShopifyHtmlConverter.ProductToJson(html, input.Locale);

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

        await _resourceService.UpdateIdentifiedContent(allItems, null);
    }

    private static IEnumerable<IdentifiedContentEntity> GetProductOptions(ProductEntity product)
    {
        var options = product.Options.SelectMany(x => x.Translations.Select(y => new IdentifiedContentEntity(y)
        {
            Id = x.Id
        })).ToArray();

        return options.Any()
            ? options
            : product.Options.Select(x => new IdentifiedContentEntity()
            {
                Id = x.Id,
                Key = "name",
                Value = x.Name
            });
    }

    private static IEnumerable<IdentifiedContentEntity>? GetProductOptionValues(ProductEntity product)
    {
        var values = product.Options.SelectMany(x => x.OptionValues.SelectMany(y => y.Translations.Select(x =>
            new IdentifiedContentEntity(x)
            {
                Id = y.Id
            }))).ToArray();

        return values.Any()
            ? values
            : product.Options.SelectMany(x => x.OptionValues.Select(x => new IdentifiedContentEntity()
            {
                Id = x.Id,
                Key = "name",
                Value = x.Name
            }));
    }

    private async Task<ProductEntity> GetProductInfo(string productId, string locale)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.Product,
            Variables = new
            {
                resourceId = productId,
                locale
            }
        };

        var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);
        return response.Product;
    }

    private async Task<IEnumerable<IdentifiedContentEntity>?> GetProductMetafields(string productId, string locale, bool outdated = false)
    {
        var productMetaFields = await GetProductMetafields(productId);
        var metaFields = await _resourceService.ListTranslatableResources(TranslatableResource.METAFIELD, locale, outdated);

        var resources = metaFields
            .Where(x => productMetaFields.Any(y => x.ResourceId == y.Id))
            .ToArray();

        var content = resources.All(x => !x.Translations.Any())
            ? resources.Select(x => (x.ResourceId, x.TranslatableContent.FirstOrDefault())).ToArray()
            : resources.Select(x => (x.ResourceId, x.Translations.FirstOrDefault())).ToArray();

        return content.Select(x => new IdentifiedContentEntity(x.Item2) { Id = x.ResourceId });
    }

    private async Task<ICollection<MetafieldEntity>> GetProductMetafields(string productId)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceId"] = productId
        };

        var client = new ShopifyClient(Creds, ShopifyClient.GenerateApiUrl(Creds, "unstable"));
        return await client.Paginate<MetafieldEntity, MetafieldPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            variables
        );
    }
}
