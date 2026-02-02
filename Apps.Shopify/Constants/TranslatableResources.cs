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
    public const string StoreResources = "Resources";

    public static readonly IEnumerable<string> SupportedContentTypes = [
        Collection,
        Article,
        Metafield,
        Blog,
        Page,
        Theme,
        Product
    ];

    public static readonly IEnumerable<string> SupportedPollingContentTypes = [
        Collection,
        Article,
        Blog,
        Page,
        Product
    ];
}
