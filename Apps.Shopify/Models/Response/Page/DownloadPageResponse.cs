using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Page;

public class DownloadPageResponse(FileReference file)
{
    [Display("Page")]
    public FileReference File { get; set; } = file;
}
