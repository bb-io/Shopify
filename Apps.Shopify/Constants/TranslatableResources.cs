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
    public const string DeliveryMethodDefinition = "Delivery method definition";
    public const string EmailTemplate = "Email template";
    public const string StoreThemeJsonTemplate = "Store theme JSON template";
    public const string Filter = "Filter";
    public const string MediaImage = "Media image";
    public const string StoreThemeAppEmbed = "Store theme app embed";
    public const string Metaobject = "Metaobject";
    public const string StoreThemeLocaleContent = "Store theme locale content";
    public const string StoreThemeSectionGroup = "Store theme section group";
    public const string StoreThemeSettingsCategory = "Store theme settings category";

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
        [Menu] = TranslatableResource.MENU,
        [DeliveryMethodDefinition] = TranslatableResource.DELIVERY_METHOD_DEFINITION,
        [EmailTemplate] = TranslatableResource.EMAIL_TEMPLATE,
        [StoreThemeJsonTemplate] = TranslatableResource.ONLINE_STORE_THEME_JSON_TEMPLATE,
        [Filter] = TranslatableResource.FILTER,
        [MediaImage] = TranslatableResource.MEDIA_IMAGE,
        [StoreThemeAppEmbed] = TranslatableResource.ONLINE_STORE_THEME_APP_EMBED,
        [Metaobject] = TranslatableResource.METAOBJECT,
        [StoreThemeLocaleContent] = TranslatableResource.ONLINE_STORE_THEME_LOCALE_CONTENT,
        [StoreThemeSectionGroup] = TranslatableResource.ONLINE_STORE_THEME_SECTION_GROUP,
        [StoreThemeSettingsCategory] = TranslatableResource.ONLINE_STORE_THEME_SETTINGS_CATEGORY,
    };
    
    private static readonly Dictionary<TranslatableResource, string> FriendlyNames = ApiTypes.ToDictionary(x => x.Value, x => x.Key);

    public static readonly List<string> SupportedContentTypes = [
        Collection,
        Article,
        Metafield,
        Blog,
        Page,
        Theme,
        Product,
        Menu,
        DeliveryMethodDefinition,
        EmailTemplate,
        StoreThemeJsonTemplate,
        Filter,
        MediaImage,
        StoreThemeAppEmbed,
        Metaobject,
        StoreThemeLocaleContent,
        StoreThemeSectionGroup,
        StoreThemeSettingsCategory,
    ];

    public static readonly List<string> SupportedPollingContentTypes = [
        Collection,
        Article,
        Blog,
        Page,
        Product,
        Menu,
        DeliveryMethodDefinition,
        EmailTemplate,
        StoreThemeJsonTemplate,
        Filter,
        MediaImage,
        StoreThemeAppEmbed,
        Metaobject,
        StoreThemeLocaleContent,
        StoreThemeSectionGroup,
        StoreThemeSettingsCategory,
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
    
    public static string Normalize(string? contentType)
    {
        TranslatableResource apiType = GetApiType(contentType);
        return FriendlyNames.TryGetValue(apiType, out var name) ? name : apiType.ToString();
    }
}