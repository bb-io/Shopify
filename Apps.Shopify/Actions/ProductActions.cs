using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Product;
using Apps.Shopify.Models.Identifiers;
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
        input.ValidateDates();

        string? query = new QueryBuilder()
            .AddContains("title", input.TitleContains)
            .AddEquals("status", input.Status)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .AddDateRange("published_at", input.PublishedAfter, input.PublishedBefore)
            .Build();

        var variables = new Dictionary<string, object>();
        string graphQlQuery = GraphQlQueries.Products;
        if (!string.IsNullOrEmpty(input.MetafieldKey))
        {
            variables["metafieldKey"] = input.MetafieldKey;
            graphQlQuery = GraphQlQueries.ProductsWithMetafields;
        }

        var response = await Client.Paginate<ProductEntity, ProductsPaginationResponse>(
            graphQlQuery, 
            QueryHelper.QueryToDictionary(query, variables)
        );

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
        [ActionParameter] ProductIdentifier resourceRequest,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] DownloadProductRequest input,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest)
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