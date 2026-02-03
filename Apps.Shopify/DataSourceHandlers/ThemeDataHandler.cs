using Apps.Shopify.Invocables;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities.Theme;
using Apps.Shopify.Models.Response.Theme;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ThemeDataHandler(InvocationContext context) : ShopifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var response = await Client.Paginate<ThemeEntity, ThemesPaginationResponse>(
            GraphQlQueries.Themes,
            [],
            cancellationToken
        );

        if (!string.IsNullOrEmpty(context.SearchString))
            response = response.Where(x => x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)).ToList();

        var items = response.Select(x => new DataSourceItem(x.Id, x.Name)).ToList();
        return items;
    }
}