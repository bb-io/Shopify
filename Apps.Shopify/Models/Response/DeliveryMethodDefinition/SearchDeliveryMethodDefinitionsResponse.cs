using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.DeliveryMethodDefinition;

public record SearchDeliveryMethodDefinitionsResponse(List<DeliveryMethodDefinitionResponse> Definitions)
{
    [Display("Delivery method definitions")]
    public List<DeliveryMethodDefinitionResponse> Definitions { get; set; } = Definitions;
}