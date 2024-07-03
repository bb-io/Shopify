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

        return new(response);
    }

    [Action("Get product content as HTML",
        Description = "Get content of a specific product in HTML format")]
    public async Task<FileResponse> GetProductTranslationContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetProductContentRequest input)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = resourceRequest.ProductId,
                locale = locale.Locale
            }
        };
        var productContent = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);

        var productInfo = input.IncludeOptions is true || input.IncludeOptionValues is true
            ? await GetProductInfo(resourceRequest.ProductId, locale.Locale)
            : new();

        var html = ShopifyHtmlConverter.ProductToHtml(new()
        {
            ProductContentEntities = productContent.TranslatableResource.Translations.Any()
                ? productContent.TranslatableResource.Translations
                : productContent.TranslatableResource.TranslatableContent,
            MetafieldsContentEntities = input.IncludeMetafields is true
                ? await GetProductMetafields(resourceRequest.ProductId, locale.Locale)
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
    public async Task UpdateProductContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
    {
        var fileStream = await FileManagementClient.DownloadAsync(file.File);
        var translations = ShopifyHtmlConverter.ProductToJson(fileStream, locale.Locale);

        await UpdateProductContent(resourceRequest.ProductId, translations.ProductContentEntities.ToList());

        if (translations.MetafieldsContentEntities != null && translations.MetafieldsContentEntities.Any())
            await UpdateProductAdditionalContents(translations.MetafieldsContentEntities.ToList());
        
        if (translations.OptionsContentEntities != null && translations.OptionsContentEntities.Any())
            await UpdateProductAdditionalContents(translations.OptionsContentEntities.ToList());
        
        if (translations.OptionValuesContentEntities != null && translations.OptionValuesContentEntities.Any())
            await UpdateProductAdditionalContents(translations.OptionValuesContentEntities.ToList());
    }
    
    private async Task UpdateProductContent(string productId,
        ICollection<TranslatableResourceContentRequest> productContents)
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
    
    private async Task UpdateProductAdditionalContents(ICollection<IdentifiedContentRequest> contents)
    {
        var groupedContentRequests = contents
            .GroupBy(x => x.ResourceId)
            .ToArray();

        if (groupedContentRequests.Any(x => x.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest))))
        {
            foreach (var contentRequest in groupedContentRequests)
            {
                var sourceContent = await GetResourceSourceContent(contentRequest.Key);
                contentRequest.ToList().ForEach(x =>
                    x.TranslatableContentDigest = sourceContent.TranslatableResource.TranslatableContent
                        .First(y => y.Key == x.Key).Digest);
            }
        }

        foreach (var contentRequest in groupedContentRequests)
        {
            var request = new GraphQLRequest()
            {
                Query = GraphQlMutations.TranslationsRegister,
                Variables = new
                {
                    resourceId = contentRequest.Key,
                    translations = contentRequest.Select(x => new TranslatableResourceContentRequest(x))
                }
            };

            await Client.ExecuteWithErrorHandling(request);
        }
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

    private async Task<IEnumerable<IdentifiedContentEntity>?> GetProductMetafields(string productId, string locale)
    {
        var productMetaFields = await GetProductMetafields(productId);
        var metaFields = await ListTranslatableResources(TranslatableResource.METAFIELD, locale);

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
                resourceId = productId,
                locale = locale
            }
        };

        var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);
        return response.Product;
    }
}