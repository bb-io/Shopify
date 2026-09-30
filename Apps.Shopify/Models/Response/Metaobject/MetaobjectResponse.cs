using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Metaobject;

public class MetaobjectResponse(ContentItemEntity contentEntity)
{
    [Display("Metaobject ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Metaobject name")]
    public string Name { get; set; } = contentEntity.Name;
}