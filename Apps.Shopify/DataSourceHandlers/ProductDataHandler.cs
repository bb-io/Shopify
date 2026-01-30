using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Product;
using Apps.Shopify.Models.Response.Product;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ProductDataHandler(InvocationContext invocationContext) 
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var variables = new Dictionary<string, object>() { ["query"] = $"title:*{context.SearchString}*" };
        var response =  await Client.Paginate<ProductEntity, ProductsPaginationResponse>(
            GraphQlQueries.Products, 
            variables, 
            cancellationToken
        );

        return response
            .OrderByDescending(x => x.CreatedAt)
            .Take(50)
            .Select(x => new DataSourceItem(x.Id, x.Title))
            .ToList();
    }
}