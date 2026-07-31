using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Market;
using Apps.Shopify.Models.Response.Market;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class MarketDataHandler(InvocationContext context) : ShopifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        string? query = new QueryBuilder()
            .AddContains("name", context.SearchString)
            .Build();

        var response = await Client.PaginateOnce<MarketEntity, MarketPaginationResponse>(
            GraphQlQueries.Markets,
            QueryHelper.QueryToDictionary(query),
            cancellationToken
        );

        var items = response.Select(x => new DataSourceItem(x.Id, x.Name)).ToList();
        return items;
    }
}