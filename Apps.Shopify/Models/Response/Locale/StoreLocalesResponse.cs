using Blackbird.Applications.Sdk.Common;


namespace Apps.Shopify.Models.Response.Locale;

public class StoreLocalesResponse(string primary, IEnumerable<string> secondary)
{
    [Display("Primary locale")]
    public string Primary { get; set; } = primary;

    [Display("Non-primary locales")]
    public IEnumerable<string> OtherLocales { get; set;} = secondary;
}