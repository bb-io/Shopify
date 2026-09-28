using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StoreThemeJsonTemplateIdentifier
{
    [Display("Store theme JSON template ID"), DataSource(typeof(ThemeJsonTemplateDataHandler))]
    public string StoreThemeJsonTemplateId { get; set; } = string.Empty;
}