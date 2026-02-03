using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Metafield;

public class UploadMetafieldRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Metafield ID"), DataSource(typeof(MetafieldDataHandler))]
    public string? MetafieldId { get; set; }
}
