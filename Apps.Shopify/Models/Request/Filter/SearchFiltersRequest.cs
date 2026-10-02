using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Filter;

public class SearchFiltersRequest
{
    [Display("Filter name contains")]
    public string? NameContains { get; set; }
}