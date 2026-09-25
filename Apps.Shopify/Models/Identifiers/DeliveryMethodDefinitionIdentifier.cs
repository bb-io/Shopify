using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Identifiers;

public class DeliveryMethodDefinitionIdentifier
{
    [Display("Delivery method definition ID")]
    public string DeliveryMethodDefinitionId { get; set; } = string.Empty;
}