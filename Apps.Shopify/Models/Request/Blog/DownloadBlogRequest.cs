using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Blog;

public class DownloadBlogRequest
{
    [Display("Include blog posts")]
    public bool? IncludeBlogPosts { get; set; }
}
