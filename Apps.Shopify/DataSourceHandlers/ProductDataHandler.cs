using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
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
        string? query = new QueryBuilder()
            .AddContains("title", context.SearchString)
            .Build();
        
        var response =  await Client.PaginateOnce<ProductEntity, ProductsPaginationResponse>(
            GraphQlQueries.Products, 
            QueryHelper.QueryToDictionary(query), 
            cancellationToken
        );

        return response
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DataSourceItem(x.Id, x.Title))
            .ToList();
    }
}