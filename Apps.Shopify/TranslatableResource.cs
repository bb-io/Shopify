namespace Apps.Shopify;

public enum TranslatableResource
{
    COLLECTION,
    METAFIELD,
    ARTICLE,
    BLOG,
    ONLINE_STORE_MENU,
    PAGE,
    ONLINE_STORE_THEME,
    PRODUCT,
    SHOP,
    SHOP_POLICY,
    
    // Do not delete - we TryParse some string inputs to these
    METAOBJECT,
    LINK
}