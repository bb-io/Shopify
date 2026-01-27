using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Product;

public class UploadProductRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Product ID"), DataSource(typeof(ProductDataHandler))]
    public string? ProductId { get; set; }
}
