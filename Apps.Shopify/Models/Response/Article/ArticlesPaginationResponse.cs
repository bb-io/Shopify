using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Article;

public class ArticlesPaginationResponse : IRestPaginationResponse<OnlineStoreArticleEntity>
{
    [JsonProperty("articles")]
    public IEnumerable<OnlineStoreArticleEntity> Items { get; set; }
}