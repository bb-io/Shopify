using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Filter;

public class FilterResponse(ContentItemEntity contentEntity)
{
    [Display("Filter ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Filter name")]
    public string Name { get; set; } = contentEntity.Name;
}