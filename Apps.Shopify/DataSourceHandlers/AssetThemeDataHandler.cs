using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreTheme;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;

namespace Apps.Shopify.DataSourceHandlers;

public class AssetThemeDataHandler(
    InvocationContext invocationContext,
    [ActionParameter] OnlineStoreThemeRequest request,
    [ActionParameter] LocaleRequest localeRequest)
    : ShopifyInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.OnlineStoreThemeId))
        {
            throw new InvalidOperationException("Please provide an online store theme ID first.");
        }

        if (string.IsNullOrEmpty(localeRequest.Locale))
        {
            throw new InvalidOperationException("Please provide a locale first.");
        }

        var translatableContent = await GetTranslatableContent(request.OnlineStoreThemeId, localeRequest.Locale, false);
        var list = translatableContent
            .Select(BuildName)
            .Where(x =>
                context.SearchString == null ||
                x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();
        
        return list.ToDictionary(x => x, x => x);
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
    
    private async Task<List<IdentifiedContentEntity>> GetTranslatableContent(string resourceId, string locale, bool outdated)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceTranslationKeys,
            Variables = new
            {
                resourceId, locale, outdated
            }
        };
        
        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        return response.TranslatableResource.GetTranslatableContent()
            .Select(x => new IdentifiedContentEntity(x)
            {
                Id = resourceId
            }).ToList();
    }

}