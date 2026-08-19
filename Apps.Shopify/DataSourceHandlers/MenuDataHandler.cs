using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Menu;
using Apps.Shopify.Models.Response.Menu;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class MenuDataHandler(InvocationContext context) : ShopifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        string? query = new QueryBuilder()
            .AddContains("title", context.SearchString)
            .Build();

        var response = await Client.PaginateOnce<MenuEntity, MenusPaginationResponse>(
            GraphQlQueries.Menus,
            QueryHelper.QueryToDictionary(query),
            cancellationToken);

        var items = response.Select(x => new DataSourceItem(x.Id, x.Title)).ToList();
        return items;
    }
}