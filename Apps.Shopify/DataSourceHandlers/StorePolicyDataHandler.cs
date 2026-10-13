using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Response.StorePolicy.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;

namespace Apps.Shopify.DataSourceHandlers;

public class StorePolicyDataHandler(InvocationContext context) : ShopifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest(GraphQlQueries.ShopPolicies, cancellationToken);
        var result = await Client.ExecuteWithErrorHandling<StorePolicyOuterResponse>(request, cancellationToken);
        
        return result.Store.StorePolicies
            .Where(x => string.IsNullOrEmpty(context.SearchString) ||
                        x.Title.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.Id, x.Title))
            .ToList();
    }
}