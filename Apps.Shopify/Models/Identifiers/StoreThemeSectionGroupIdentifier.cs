using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StoreThemeSectionGroupIdentifier
{
    [Display("Store theme section group ID"), DataSource(typeof(ThemeSectionGroupDataHandler))]
    public string StoreThemeSectionGroupId { get; set; } = string.Empty;
}