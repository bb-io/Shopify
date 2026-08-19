using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Menu;

public class DownloadMenuResponse(FileReference file)
{
    [Display("Menu")]
    public FileReference File { get; set; } = file;
}