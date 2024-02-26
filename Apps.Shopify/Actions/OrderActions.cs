using Apps.Shopify.Api;
using Apps.Shopify.Invocables;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Shopify.Actions;

public class OrderActions : ShopifyInvocable
{
    public OrderActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    [Action("Search orders", Description = "Search for orders based on provided criterias")]
    public async Task<ListOrdersResponse> SearchOrders([ActionParameter] SearchOrdersRequest input)
    {
        var request = new ShopifyRequest("orders.json", Method.Get, Creds);
        var response = await Client.Paginate<OrderEntity, OrderPaginationResponse>(request);

        return new(response);
    }

    [Action("Get order", Description = "Get details of a specific order")]
    public async Task<OrderEntity> GetOrder([ActionParameter] OrderRequest input)
    {
        var request = new ShopifyRequest("orders.json", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<OrderResponse>(request);

        return response.Order;
    }
    
    [Action("Create order", Description = "Create a new order")]
    public async Task<OrderEntity> CreateOrder([ActionParameter] CreateOrderRequest input)
    {
        var request = new ShopifyRequest("orders.json", Method.Post, Creds);
        var response = await Client.ExecuteWithErrorHandling<OrderResponse>(request);

        return response.Order;
    }
    
    [Action("Delete order", Description = "Delete specific order")]
    public Task DeleteOrder([ActionParameter] OrderRequest input)
    {
        var request = new ShopifyRequest("orders.json", Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling<OrderResponse>(request);
    }
}