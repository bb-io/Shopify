using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Response.Metafield;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class MetafieldDataHandler(InvocationContext invocationContext) 
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var variables = new Dictionary<string, object>()
        {
            ["ownerType"] = "PRODUCT"
        };

        var response = await Client.Paginate<MetafieldDefinitionEntity, MetafieldDefinitionPaginationResponse>(
            GraphQlQueries.MetafieldDefinitions, 
            variables, 
            cancellationToken
        );

        return response
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .Select(x => new DataSourceItem(x.Id, x.Name))
            .ToList();
    }
}
