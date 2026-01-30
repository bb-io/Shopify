using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class ThemeIdentifier
{
    [Display("Online store theme ID")]
    [DataSource(typeof(ThemeDataHandler))]
    public string ThemeId { get; set; }
}