using Apps.Shopify.Api;
using Apps.Shopify.Invocables;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Shopify.Actions;

public class ProductActions : ShopifyInvocable
{
    public ProductActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search products", Description = "Search for products based on provided criterias")]
    public async Task<ListProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest input)
    {
        var request = new ShopifyRequest("products.json", Method.Get, Creds);
        var response = await Client.Paginate<ProductEntity, ProductPaginationResponse>(request);

        return new(response);
    }

    [Action("Get product", Description = "Get details of a specific product")]
    public async Task<ProductEntity> GetProduct([ActionParameter] ProductRequest input)
    {
        var request = new ShopifyRequest("products.json", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);

        return response.Product;
    }

    [Action("Create product", Description = "Create a new product")]
    public async Task<ProductEntity> CreateProduct([ActionParameter] CreateProductRequest input)
    {
        var request = new ShopifyRequest("products.json", Method.Post, Creds);
        var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);

        return response.Product;
    }

    [Action("Delete product", Description = "Delete specific product")]
    public Task DeleteProduct([ActionParameter] ProductRequest input)
    {
        var request = new ShopifyRequest("products.json", Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }
}