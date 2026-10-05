using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StoreThemeAppEmbedIdentifier
{
    [Display("Store theme app embed ID"), DataSource(typeof(ThemeAppEmbedDataHandler))]
    public string StoreThemeAppEmbedId { get; set; } = string.Empty;
}