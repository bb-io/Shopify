using GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Response.Locale;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class LanguageDataHandler(InvocationContext invocationContext) 
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest() { Query = GraphQlQueries.Locales };
        var response = await Client.ExecuteWithErrorHandling<ShopLocalesResponse>(request, cancellationToken);

        return response.ShopLocales
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.Locale, x.Name));
    }
}