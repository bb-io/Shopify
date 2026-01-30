using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Response.Page;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class PageDataHandler(InvocationContext context) : ShopifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        string? query = new QueryBuilder()
            .AddContains("title", context.SearchString)
            .Build();

        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(
            GraphQlQueries.Pages,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new DataSourceItem(x.Id, x.Title)).ToList();
        return items;
    }
}