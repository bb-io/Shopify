using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StorePolicy;

public class SearchStorePoliciesRequest
{
    [Display("Store policy name contains")]
    public string? NameContains { get; set; }
}