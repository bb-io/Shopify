using Blackbird.Applications.Sdk.Common;


namespace Apps.Shopify.Models.Response.Locale;

public class StoreLocalesResponse
{
    [Display("Primary locale")]
    public ShopLocale Primary { get; set; }

    [Display("Non-primary locales")]
    public IEnumerable<ShopLocale> OtherLocales { get; set;}
}