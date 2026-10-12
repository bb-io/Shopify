using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.SellingPlan;

public class UploadSellingPlanRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Selling plan ID"), DataSource(typeof(SellingPlanDataHandler))]
    public string? SellingPlanId { get; set; }
}