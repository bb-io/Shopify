using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Collection;
using Apps.Shopify.Models.Response.Collection;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class CollectionDataHandler(InvocationContext context) : ShopifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        string? query = new QueryBuilder()
            .AddContains("title", context.SearchString)
            .Build();

        var response = await Client.Paginate<CollectionEntity, CollectionsPaginationResponse>(
            GraphQlQueries.Collections,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new DataSourceItem(x.Id, x.Title)).ToList();
        return items;
    }
}