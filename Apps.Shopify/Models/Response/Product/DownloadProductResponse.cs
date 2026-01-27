using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Product;

public class DownloadProductResponse(FileReference file)
{
    [Display("Product")]
    public FileReference File { get; set; } = file;
}
