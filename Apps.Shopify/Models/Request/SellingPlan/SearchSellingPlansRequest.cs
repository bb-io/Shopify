using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.SellingPlan;

public class SearchSellingPlansRequest
{
    [Display("Search plan name contains")]
    public string? NameContains { get; set; }
}