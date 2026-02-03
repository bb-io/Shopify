using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStore;

public class UploadStoreContentRequest
{
    [Display("Content")]
    public FileReference Content { get; set; }
}
