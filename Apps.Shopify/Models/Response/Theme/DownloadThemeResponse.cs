using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Theme;

public class DownloadThemeResponse(FileReference file)
{
    [Display("Theme")]
    public FileReference File { get; set; } = file;
}
