using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Product;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

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

    [Action("Get product content as HTML", Description = "Get content of a specific product in HTML format")]
    public Task<FileResponse> GetProductContent([ActionParameter] ProductRequest input)
        => GetResourceContent(input.ProductId);

    [Action("Get product translation content as HTML",
        Description = "Get content of a specific product translation in HTML format")]
    public Task<FileResponse> GetProductTranslationContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleRequest locale)
        => GetResourceTranslationContent(resourceRequest.ProductId, locale.Locale);

    [Action("Update product content from HTML", Description = "Update content of a specific product from HTML file")]
    public Task UpdateProductContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.ProductId, locale.Locale, file.File);
}