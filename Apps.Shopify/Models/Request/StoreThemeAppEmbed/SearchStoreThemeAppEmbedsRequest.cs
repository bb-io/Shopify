using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StoreThemeAppEmbed;

public class SearchStoreThemeAppEmbedsRequest
{
    [Display("Store theme app embed name contains")]
    public string? NameContains { get; set; }
}