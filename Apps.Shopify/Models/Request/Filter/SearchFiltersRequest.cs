using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Filter;

public class SearchFiltersRequest
{
    [Display("Delivery method definition name contains")]
    public string? NameContains { get; set; }
}