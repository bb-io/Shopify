using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StorePolicy;

public class StorePolicyResponse(ContentItemEntity contentEntity)
{
    [Display("Policy ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Policy name")]
    public string Name { get; set; } = contentEntity.Name;
}