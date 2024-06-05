using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Metafield;

public class MetafieldRequest
{
    [Display("Metafield ID")]
    [DataSource(typeof(MetafieldDataSourceHandler))]
    public string MetafieldId { get; set; }
}