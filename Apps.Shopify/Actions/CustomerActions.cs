using Apps.Shopify.Api;
using Apps.Shopify.Invocables;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Shopify.Actions;

[ActionList]
public class CustomerActions : ShopifyInvocable
{
    public CustomerActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search customers", Description = "Search for customers based on provided criterias")]
    public async Task<ListCustomersResponse> SearchCustomers([ActionParameter] SearchCustomersRequest input)
    {
        var request = new ShopifyRequest("customers.json", Method.Get, Creds);
        var response = await Client.Paginate<CustomerEntity, CustomerPaginationResponse>(request);

        return new(response);
    }

    [Action("Get customer", Description = "Get details of a specific customer")]
    public async Task<CustomerEntity> GetCustomer([ActionParameter] CustomerRequest input)
    {
        var request = new ShopifyRequest("customers.json", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<CustomerResponse>(request);

        return response.Customer;
    }
    
    [Action("Create customer", Description = "Create a new customer")]
    public async Task<CustomerEntity> CreateCustomer([ActionParameter] CreateCustomerRequest input)
    {
        var request = new ShopifyRequest("customers.json", Method.Post, Creds);
        var response = await Client.ExecuteWithErrorHandling<CustomerResponse>(request);

        return response.Customer;
    }
    
    [Action("Delete customer", Description = "Delete specific customer")]
    public Task DeleteCustomer([ActionParameter] CustomerRequest input)
    {
        var request = new ShopifyRequest("customers.json", Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling<CustomerResponse>(request);
    }
}