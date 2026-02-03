using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Collection;

public class DownloadCollectionResponse(FileReference file)
{
    [Display("Collection")]
    public FileReference File { get; set; } = file;
}
