using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.DeliveryMethodDefinition;

public class SearchDeliveryMethodDefinitionsRequest
{
    [Display("Delivery method definition name contains")]
    public string? NameContains { get; set; }
}