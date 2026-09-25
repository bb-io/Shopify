using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class DeliveryMethodDefinitionIdentifier
{
    [Display("Delivery method definition ID"), DataSource(typeof(DeliveryMethodDefinitionDataHandler))]
    public string DeliveryMethodDefinitionId { get; set; } = string.Empty;
}