using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Blog;

public class DownloadBlogRequest
{
    [Display("Include blog posts", Description = "False by default")]
    public bool? IncludeBlogPosts { get; set; }
}
