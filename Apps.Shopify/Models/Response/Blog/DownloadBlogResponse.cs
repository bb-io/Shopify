using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Blog;

public class DownloadBlogResponse(FileReference file)
{
    [Display("Blog")]
    public FileReference File { get; set; } = file;
}
