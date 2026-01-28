using Apps.Shopify.Models.Entities.Article;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Article;

public class GetArticleResponse(ArticleEntity entity)
{
    [Display("ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Title")]
    public string Title { get; set; } = entity.Title;

    [Display("Created at")]
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;

    [Display("Updated at")]
    public DateTime? UpdatedAt { get; set; } = entity.UpdatedAt;

    [Display("Author")]
    public string Author { get; set; } = entity.Author.Name;

    [Display("Handle")]
    public string Handle { get; set; } = entity.Handle;
}
