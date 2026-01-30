using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;

namespace Apps.Shopify.DataSourceHandlers;

public class AssetThemeDataHandler(
    InvocationContext invocationContext,
    [ActionParameter] ThemeIdentifier request,
    [ActionParameter] DownloadContentRequest contentRequest)
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var themeId = !string.IsNullOrEmpty(request.ThemeId)
            ? request.ThemeId
            : contentRequest.ContentId;

        if (string.IsNullOrEmpty(themeId))
            throw new PluginMisconfigurationException("Please select a store theme first.");

        var assets = await GetTranslatableContent(themeId);

        return assets
            .Select(BuildName)
            .Where(x => context.SearchString == null || x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .Take(50)
            .Select(x => new DataSourceItem(x, x));
    }

    private static string BuildName(IdentifiedContentEntity entity)
    {
        var key = entity.Key;
        if (key.Contains(".json"))
        {
            return key.Substring(0, key.IndexOf(".json", StringComparison.Ordinal));
        }

        var parts = key.Split('.');
        if (parts.Length >= 2)
        {
            return string.Join('.', parts.Take(2));
        }

        return key;
    }

    private async Task<List<IdentifiedContentEntity>> GetTranslatableContent(string resourceId)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceIds,
            Variables = new { resourceId }
        };

        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        var content = response.TranslatableResource.TranslatableContent ?? [];

        return content
            .Select(x => new IdentifiedContentEntity(x)
            {
                Id = resourceId
            }).ToList();
    }
}