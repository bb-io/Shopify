using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeSectionGroup;

public class StoreThemeSectionGroupResponse(ContentItemEntity contentEntity)
{
    [Display("Theme section group ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Theme section group name")]
    public string Name { get; set; } = contentEntity.Name;
}