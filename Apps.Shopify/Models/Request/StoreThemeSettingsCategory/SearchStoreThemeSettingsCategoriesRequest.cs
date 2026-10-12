using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StoreThemeSettingsCategory;

public class SearchStoreThemeSettingsCategoriesRequest
{
    [Display("Store theme settings category name contains")]
    public string? NameContains { get; set; }
}