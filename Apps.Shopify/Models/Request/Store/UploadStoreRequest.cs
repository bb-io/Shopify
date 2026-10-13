using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Store;

public class UploadStoreRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
}