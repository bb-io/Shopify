using Apps.Shopify.Api;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Models.Response.Product;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search products", Description = "Search for products based on provided criterias")]
    public async Task<ListProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest input)
    {
        if ((input.MetafieldKey == null) ^ (input.MetafieldValue == null))
            throw new PluginMisconfigurationException("Metafield and Metafield value should be both either filled or empty");
        
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

    [Action("Download product", Description = "Get content of a specific product")]
    public async Task<DownloadProductResponse> GetProductTranslationContent(
        [ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] GetProductContentRequest input,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.PRODUCT);
        var request = new DownloadContentRequest
        {
            ContentId = resourceRequest.ProductId,
            IncludeOptions = input.IncludeOptions,
            IncludeMetafields = input.IncludeMetafields,
            IncludeOptionValues = input.IncludeOptionValues,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload product", Description = "Upload content of a specific product")]
    public async Task UpdateProductContent(
        [ActionParameter] UploadProductRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.PRODUCT);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = input.ProductId,
            Locale = locale.Locale
        };

        await service.Upload(request);
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