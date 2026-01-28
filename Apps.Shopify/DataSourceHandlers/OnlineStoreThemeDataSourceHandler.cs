using Apps.Shopify.Api.Rest;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Response.Theme;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Shopify.DataSourceHandlers;

public class OnlineStoreThemeDataSourceHandler : ShopifyInvocable, IAsyncDataSourceHandler
{

    public OnlineStoreThemeDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ShopifyRestRequest("themes.json", Method.Get, Creds);
        var response = await RestClient.ExecuteWithErrorHandling<SearchThemesResponse>(request);
        
        return response.Themes
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.UpdatedAt)
            .Take(50)
            .ToDictionary(x => $"gid://shopify/OnlineStoreTheme/{x.Id}", x => x.Name);
    }
}