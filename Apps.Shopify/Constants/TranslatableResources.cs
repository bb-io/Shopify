namespace Apps.Shopify.Constants;

public static class TranslatableResources
{
    public static readonly IEnumerable<string> SupportedContentTypes = [
        TranslatableResource.COLLECTION.ToString(),
        TranslatableResource.METAFIELD.ToString(),
        TranslatableResource.ARTICLE.ToString(),
        TranslatableResource.BLOG.ToString(),
        TranslatableResource.PAGE.ToString(),
        TranslatableResource.ONLINE_STORE_THEME.ToString(),
        TranslatableResource.PRODUCT.ToString()
    ];

    public static readonly IEnumerable<string> SupportedPollingContentTypes = [
        TranslatableResource.COLLECTION.ToString(),
        TranslatableResource.ARTICLE.ToString(),
        TranslatableResource.BLOG.ToString(),
        TranslatableResource.PAGE.ToString(),
        TranslatableResource.PRODUCT.ToString()
    ];
}
