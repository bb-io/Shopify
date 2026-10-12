using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class SellingPlanIdentifier
{
    [Display("Selling plan ID"), DataSource(typeof(SellingPlanDataHandler))]
    public string SellingPlanId { get; set; } = string.Empty;
}