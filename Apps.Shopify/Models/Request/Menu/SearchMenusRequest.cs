using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Menu;

public class SearchMenusRequest
{
    [Display("Menu name contains")]
    public string? NameContains { get; set; }
}