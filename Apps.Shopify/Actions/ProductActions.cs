using System.Text;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Response.Product;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.System;

namespace Apps.Shopify.Actions;

public class ProductActions : ShopifyInvocable
{
    public ProductActions(InvocationContext invocationContext) : base(invocationContext)
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

    // [Action("Get product content as HTML", Description = "Get content of a specific product in HTML format")]
    // public async Task<ProductEntity> GetProductContent([ActionParameter] ProductRequest input)
    // {
    //     var request = new ShopifyRequest("products.json", Method.Get, Creds);
    //     var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    //
    //     return response.Product;
    // }
    //
    // [Action("Update product content from HTML", Description = "Update content of a specific product from HTML file")]
    // public async Task<ProductEntity> UpdateProductContent([ActionParameter] CreateProductRequest input)
    // {
    //     var request = new ShopifyRequest("products.json", Method.Post, Creds);
    //     var response = await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    //
    //     return response.Product;
    // }
}