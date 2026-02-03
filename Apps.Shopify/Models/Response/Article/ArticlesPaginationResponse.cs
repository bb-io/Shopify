using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Article;

public class ArticlesPaginationResponse : IPaginationResponse<ArticleEntity>
{
    [JsonProperty("articles")]
    public PaginationData<ArticleEntity> Items { get; set; }
}