using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class MetafieldDefinitionIdentifier
{
    [Display("Metafield definition ID")]
    [DataSource(typeof(ProductMetafieldDefinitionDataHandler))]
    public string MetafieldDefinitionId { get; set; }
}