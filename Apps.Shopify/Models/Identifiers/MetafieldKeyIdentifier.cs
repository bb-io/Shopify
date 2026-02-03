using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class MetafieldKeyIdentifier
{
    [Display("Metafield key"), DataSource(typeof(ProductMetafieldKeyDataHandler))]
    public string MetafieldKey { get; set; }
}
