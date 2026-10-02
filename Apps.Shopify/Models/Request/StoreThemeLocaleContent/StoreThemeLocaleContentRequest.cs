using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StoreThemeLocaleContent;

public class StoreThemeLocaleContentRequest
{
    [Display("Store theme locale content name contains")]
    public string? NameContains { get; set; }
}