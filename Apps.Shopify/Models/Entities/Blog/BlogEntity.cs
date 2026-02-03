using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Blog;

public class BlogEntity
{
    [Display("Blog ID")]
    public string Id { get; set; }

    [Display("Blog title")]
    public string Title { get; set; }

    [Display("Created at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime? UpdatedAt { get; set; }
}
