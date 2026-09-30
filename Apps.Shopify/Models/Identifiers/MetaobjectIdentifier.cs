using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class MetaobjectIdentifier
{
    [Display("Metaobject ID"), DataSource(typeof(MetaobjectDataHandler))]
    public string MetaobjectId { get; set; } = string.Empty;
}