using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Api;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Models.Response.Product;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions;

[ActionList]
public class ProductActions : TranslatableResourceActions
{
    public ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("Search products", Description = "Search for products based on provided criterias")]
    public async Task<ListProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest input)
    {
        if ((input.MetafieldKey == null) ^ (input.MetafieldValue == null))
            throw new("Metafield and Metafield value should be both either filled or empty");
        
        var variables = new Dictionary<string, object>();

        var query = new List<string>();
        if (input.Status is not null)
        {
            query.Add($"status:{input.Status}");
        }

        if (!string.IsNullOrWhiteSpace(query.ToString()))
        {
            variables["query"] = string.Join(" AND ", query);
        }

        var response = await Client
            .Paginate<ProductEntity, ProductsPaginationResponse>(GraphQlQueries.Products, variables);

        if (input.MetafieldKey is not null)
            await FilterProductsByMetafields(response, input.MetafieldKey, input.MetafieldValue!);

        return new(response);
    }

    [Action("Get product content as HTML",
        Description = "Get content of a specific product in HTML format")]
    public async Task<FileResponse> GetProductTranslationContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetProductContentRequest input,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = resourceRequest.ProductId,
                locale = locale.Locale,
                outdated = getContentRequest.Outdated ?? false
            }
        };
        var productContent = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);

        var productInfo = input.IncludeOptions is true || input.IncludeOptionValues is true
            ? await GetProductInfo(resourceRequest.ProductId, locale.Locale)
            : new();

        var html = ShopifyHtmlConverter.ProductToHtml(new()
        {
            ProductContentEntities = productContent.TranslatableResource.GetTranslatableContent()
                .Select(x => new IdentifiedContentEntity(x)
                {
                    Id = resourceRequest.ProductId
                }),
            MetafieldsContentEntities = input.IncludeMetafields is true
                ? await GetProductMetafields(resourceRequest.ProductId, locale.Locale,
                    getContentRequest.Outdated ?? default)
                : [],
            OptionsContentEntities = input.IncludeOptions is true
                ? GetProductOptions(productInfo)
                : [],
            OptionValuesContentEntities = input.IncludeOptionValues is true
                ? GetProductOptionValues(productInfo)
                : [],
        });

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"{resourceRequest.ProductId.GetShopifyItemId()}.html")
        };
    }

    [Action("Update product content from HTML", Description = "Update content of a specific product from HTML file")]
    public async Task UpdateProductContent([ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
    {
        var fileStream = await FileManagementClient.DownloadAsync(file.File);
        var translations = ShopifyHtmlConverter.ProductToJson(fileStream, locale.Locale);

        var productContent = translations.ProductContentEntities.ToList();

        await UpdateProductContent(productContent.First().ResourceId, productContent);

        await UpdateIdentifiedContent(translations.MetafieldsContentEntities?.ToList());
        await UpdateIdentifiedContent(translations.OptionsContentEntities?.ToList());
        await UpdateIdentifiedContent(translations.OptionValuesContentEntities?.ToList());
    }

    private async Task UpdateProductContent(string productId,
        ICollection<IdentifiedContentRequest> productContents)
    {
        if (productContents.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
        {
            var sourceContent = await GetResourceSourceContent(productId);
            productContents.ToList().ForEach(x =>
                x.TranslatableContentDigest = sourceContent.TranslatableResource.TranslatableContent
                    .First(y => y.Key == x.Key).Digest);
        }

        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.TranslationsRegister,
            Variables = new
            {
                resourceId = productId,
                translations = productContents.Select(x => new TranslatableResourceContentRequest(x))
            }
        };

        await Client.ExecuteWithErrorHandling(request);
    }

    private IEnumerable<IdentifiedContentEntity> GetProductOptions(ProductEntity product)
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

    private IEnumerable<IdentifiedContentEntity>? GetProductOptionValues(ProductEntity product)
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

    private async Task<IEnumerable<IdentifiedContentEntity>?> GetProductMetafields(string productId, string locale,
        bool outdated = false)
    {
        var productMetaFields = await GetProductMetafields(productId);
        var metaFields = await ListTranslatableResources(TranslatableResource.METAFIELD, locale, outdated);

        var resources = metaFields
            .Where(x => productMetaFields.Any(y => x.ResourceId == y.Id))
            .ToArray();

        var content = resources.All(x => !x.Translations.Any())
            ? resources.Select(x => (x.ResourceId, x.TranslatableContent.FirstOrDefault())).ToArray()
            : resources.Select(x => (x.ResourceId, x.Translations.FirstOrDefault())).ToArray();

        return content
            .Select(x => new IdentifiedContentEntity(x.Item2)
            {
                Id = x.ResourceId
            });
    }

    private async Task<ICollection<MetafieldEntity>> GetProductMetafields(string productId)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceId"] = productId
        };

        var client = new ShopifyClient(Creds, ShopifyClient.GenerateApiUrl(Creds, "unstable"));
        return await client.Paginate<MetafieldEntity, MetafieldPaginationResponse>(GraphQlQueries.ProductMetaFields,
            variables);
    }

    private async Task<ProductEntity> GetProductInfo(string productId, string locale)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.Product,
            Variables = new
            {
                resourceId = productId, locale
            }
        };

        var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);
        return response.Product;
    }
    
    private async Task FilterProductsByMetafields(List<ProductEntity> response, string metafieldKey, string metafieldValue)
    {
        foreach (var product in response.ToArray())
        {
            var metafields = await GetProductMetafields(product.Id);
            
            if(metafields.Any(x => x.Key == metafieldKey && x.Value == metafieldValue))
                continue;

            response.Remove(product);
        }
    }
}