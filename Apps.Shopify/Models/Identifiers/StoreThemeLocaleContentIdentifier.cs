using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StoreThemeLocaleContentIdentifier
{
    [Display("Store theme locale content ID"), DataSource(typeof(ThemeLocaleContentDataHandler))]
    public string StoreThemeLocaleContentId { get; set; } = string.Empty;
}