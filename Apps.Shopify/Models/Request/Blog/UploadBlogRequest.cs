using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStoreBlog;

public class UploadBlogRequest
{
    [Display("Content")]
    public FileReference File { get; set; }
}
