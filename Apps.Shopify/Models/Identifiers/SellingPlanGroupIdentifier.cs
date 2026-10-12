using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class SellingPlanGroupIdentifier
{
    [Display("Selling plan group ID"), DataSource(typeof(SellingPlanGroupDataHandler))]
    public string SellingPlanGroupId { get; set; } = string.Empty;
}