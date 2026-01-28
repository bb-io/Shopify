using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Product;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Response.Product;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search products", Description = "Search for products based on provided criterias")]
    public async Task<SearchProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest input)
    {
        input.Validate();

        var variables = new Dictionary<string, object>();
        var query = new List<string>();

        if (!string.IsNullOrEmpty(input.TitleContains))
            query.Add($"title:*{input.TitleContains}*");

        if (!string.IsNullOrEmpty(input.Status))
            query.Add($"status:{input.Status}");

        string graphQlQuery = GraphQlQueries.Products;
        if (!string.IsNullOrEmpty(input.MetafieldKey))
        {
            variables["metafieldKey"] = input.MetafieldKey;
            graphQlQuery = GraphQlQueries.ProductsWithMetafields;
        }

        if (!string.IsNullOrWhiteSpace(query.ToString()))
            variables["query"] = string.Join(" AND ", query);

        var response = await Client.Paginate<ProductEntity, ProductsPaginationResponse>(graphQlQuery, variables);

        if (!string.IsNullOrEmpty(input.MetafieldKey))
        {
            response = response
                .Where(
                    p => p.Metafield != null &&
                    p.Metafield.Value.Contains(input.MetafieldValueContains!, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();
        }

        var result = response.Select(x => new GetProductResponse(x)).ToList();
        return new(result);
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
}