using Apps.Shopify.Models.Entities;

namespace Apps.Shopify.Models.Response.Article;

public class ArticlesPaginationResponse
{
    public IEnumerable<OnlineStoreArticleEntity> Articles { get; set; }
}