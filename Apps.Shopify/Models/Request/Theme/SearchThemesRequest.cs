using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request.Theme;

public class SearchThemesRequest
{
    [Display("Roles"), StaticDataSource(typeof(ThemeRoleDataHandler))]
    public string? Role { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }
}
