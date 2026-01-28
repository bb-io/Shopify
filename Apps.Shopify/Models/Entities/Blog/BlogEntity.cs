using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Blog;

public class BlogEntity
{
    [Display("ID")]
    public string Id { get; set; }

    [Display("Title")]
    public string Title { get; set; }
}
