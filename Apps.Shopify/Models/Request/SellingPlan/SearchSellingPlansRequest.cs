using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.SellingPlan;

public class SearchSellingPlansRequest
{
    [Display("Selling plan name contains")]
    public string? NameContains { get; set; }
}