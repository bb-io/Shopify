using Blackbird.Applications.Sdk.Common;


namespace Apps.Shopify.Models.Response.Locale;

public class StoreLocalesResponse
{
    [Display("Primary locale")]
    public string Primary { get; set; }

    [Display("Non-primary locales")]
    public IEnumerable<string> OtherLocales { get; set;}
}