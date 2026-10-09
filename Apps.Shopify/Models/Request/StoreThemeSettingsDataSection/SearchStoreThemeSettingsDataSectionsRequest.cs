using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StoreThemeSettingsDataSection;

public class SearchStoreThemeSettingsDataSectionsRequest
{
    [Display("Store theme settings data section name contains")]
    public string? NameContains { get; set; }
}