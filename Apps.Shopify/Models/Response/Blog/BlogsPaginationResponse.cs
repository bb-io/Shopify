using Apps.Shopify.Models.Entities.Blog;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Blog;

public class BlogsPaginationResponse : IPaginationResponse<BlogEntity>
{
    [JsonProperty("blogs")]
    public PaginationData<BlogEntity> Items { get; set; }
}
