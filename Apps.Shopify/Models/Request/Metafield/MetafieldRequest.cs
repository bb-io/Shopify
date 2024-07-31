using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Metafield;

public class MetafieldRequest
{
    [Display("Metafield")]
    [DataSource(typeof(ProductMetafieldDefinitionDataHandler))]
    public string MetafieldDefinitionId { get; set; }
}