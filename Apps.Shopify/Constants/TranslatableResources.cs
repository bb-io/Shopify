using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Shopify.Constants;

public static class TranslatableResources
{
    public const string Collection = "Collection";
    public const string Metafield = "Metafield";
    public const string Article = "Article";
    public const string Blog = "Blog";
    public const string Page = "Page";
    public const string Theme = "Theme";
    public const string Product = "Product";
    public const string Store = "Store";
    public const string Menu = "Menu";

    private static readonly Dictionary<string, TranslatableResource> ApiTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [Collection] = TranslatableResource.COLLECTION,
        [Metafield] = TranslatableResource.METAFIELD,
        [Article] = TranslatableResource.ARTICLE,
        [Blog] = TranslatableResource.BLOG,
        [Page] = TranslatableResource.PAGE,
        [Theme] = TranslatableResource.ONLINE_STORE_THEME,
        [Product] = TranslatableResource.PRODUCT,
        [Store] = TranslatableResource.SHOP,
        [Menu] = TranslatableResource.MENU
    };

    public static readonly List<string> SupportedContentTypes = [
        Collection,
        Article,
        Metafield,
        Blog,
        Page,
        Theme,
        Product,
        Menu
    ];

    public static readonly List<string> SupportedPollingContentTypes = [
        Collection,
        Article,
        Blog,
        Page,
        Product,
        Menu
    ];

    public static bool TryGetApiType(string? contentType, out TranslatableResource apiType)
    {
        return ApiTypes.TryGetValue(contentType?.Trim() ?? string.Empty, out apiType);
    }

    public static TranslatableResource GetApiType(string? contentType)
    {
        return TryGetApiType(contentType, out var apiType)
            ? apiType
            : throw new PluginMisconfigurationException(
                $"Unsupported content type '{contentType}'. Supported values: {string.Join(", ", SupportedContentTypes)}.");
    }
    
    public static string GetFriendlyName(TranslatableResource apiType) => FriendlyNames.TryGetValue(apiType, out var name) ? name : apiType.ToString();

    public static string Normalize(string? contentType) => GetFriendlyName(GetApiType(contentType));
    
    private static readonly Dictionary<TranslatableResource, string> FriendlyNames = ApiTypes.ToDictionary(x => x.Value, x => x.Key);
}