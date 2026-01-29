namespace Apps.Shopify.Constants;

public static class TranslatableResources
{
    public static readonly IEnumerable<string> SupportedResources = [
        TranslatableResource.COLLECTION.ToString(),
        TranslatableResource.METAFIELD.ToString(),
        TranslatableResource.ARTICLE.ToString(),
        TranslatableResource.BLOG.ToString(),
        TranslatableResource.PAGE.ToString(),
        TranslatableResource.ONLINE_STORE_THEME.ToString(),
        TranslatableResource.PRODUCT.ToString()
    ];
}
