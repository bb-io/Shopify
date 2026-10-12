using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.SellingPlanGroup;

public class UploadSellingPlanGroupRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Selling plan group ID"), DataSource(typeof(SellingPlanGroupDataHandler))]
    public string? SellingPlanGroupId { get; set; }
}