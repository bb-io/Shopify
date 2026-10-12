using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StoreThemeSettingsDataSectionsIdentifier
{
    [Display("Store theme settings data sections ID"), DataSource(typeof(ThemeSettingsDataSectionDataHandler))]
    public string StoreThemeSettingsDataSectionsId { get; set; } = string.Empty;
}