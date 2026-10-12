using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StoreThemeSettingsCategoryIdentifier
{
    [Display("Store theme settings category ID"), DataSource(typeof(ThemeSettingsCategoryDataHandler))]
    public string StoreThemeSettingsCategoryId { get; set; } = string.Empty;
}