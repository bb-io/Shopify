using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Page;

public class PageEntity
{
    [Display("Page ID")]
    public string Id { get; set; }

    [Display("Page title")]
    public string Title { get; set; }

    [Display("Created at")] 
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }

    [Display("Published at")]
    public DateTime? PublishedAt { get; set; }

    [Display("Handle")]
    public string Handle { get; set; }
}