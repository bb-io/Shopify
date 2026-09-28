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
    MENU,
    LINK,
    DELIVERY_METHOD_DEFINITION,
    EMAIL_TEMPLATE,
    ONLINE_STORE_THEME_JSON_TEMPLATE,
    
    // Do not delete - we TryParse some string inputs to these
    METAOBJECT,
}