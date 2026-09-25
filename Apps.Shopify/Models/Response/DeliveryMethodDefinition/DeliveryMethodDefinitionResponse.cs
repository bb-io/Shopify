using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.DeliveryMethodDefinition;

public class DeliveryMethodDefinitionResponse(ContentItemEntity contentEntity)
{
    [Display("Delivery method definition ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Delivery method definition name")]
    public string Name { get; set; } = contentEntity.Name;
}