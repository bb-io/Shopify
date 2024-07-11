using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Blog;

public class ListBlogsResponse
{
    public IEnumerable<Blog> Blogs {get; set;}
}

public class Blog
{
    [Display("Resource ID")]
    public string ResourceId { get; set; }

    public string Title { get; set; }
}