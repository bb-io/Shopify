using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Article;

public class ArticleEntity
{
    [Display("Article ID")]
    public string Id { get; set; }

    [Display("Article title")]
    public string Title { get; set; }

    [Display("Created at")] 
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")] 
    public DateTime? UpdatedAt { get; set; }

    [Display("Author")]
    public ArticleAuthor Author { get; set; }

    [Display("Handle")]
    public string Handle { get; set; }
}