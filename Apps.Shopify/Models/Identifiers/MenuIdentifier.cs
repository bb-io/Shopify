using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class MenuIdentifier
{
    [Display("Menu ID"), DataSource(typeof(MenuDataHandler))]
    public string MenuId { get; set; } = string.Empty;
}