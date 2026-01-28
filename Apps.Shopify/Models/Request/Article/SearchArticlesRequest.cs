using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Article;

public class SearchArticlesRequest
{
    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Published before")]
    public DateTime? PublishedBefore { get; set; }

    [Display("Published after")]
    public DateTime? PublishedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; } 
}
