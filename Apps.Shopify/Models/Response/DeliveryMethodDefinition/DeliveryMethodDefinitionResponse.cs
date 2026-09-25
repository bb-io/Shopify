using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.DeliveryMethodDefinition;

public class DeliveryMethodDefinitionResponse(ContentItemEntity contentEntity)
{
    [Display("Definition ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Definition name")]
    public string Name { get; set; } = contentEntity.Name;
}