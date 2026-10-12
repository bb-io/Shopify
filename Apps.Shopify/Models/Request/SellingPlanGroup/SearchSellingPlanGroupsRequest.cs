using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.SellingPlanGroup;

public class SearchSellingPlanGroupsRequest
{
    [Display("Selling plan group name contains")]
    public string? NameContains { get; set; }
}