using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Metafield;

public class DownloadMetafieldResponse(FileReference file)
{
    [Display("Metafield")]
    public FileReference File { get; set; } = file;
}
