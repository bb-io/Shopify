using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStoreBlog;

public class UploadBlogRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Blog ID"), DataSource(typeof(OnlineStoreBlogHandler))]
    public string? BlogId { get; set; }
}
