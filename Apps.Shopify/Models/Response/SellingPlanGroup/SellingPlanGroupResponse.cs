using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.SellingPlanGroup;

public class SellingPlanGroupResponse(ContentItemEntity contentEntity)
{
    [Display("Plan group ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Plan group name")]
    public string Name { get; set; } = contentEntity.Name;
}