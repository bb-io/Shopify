using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Page;

public class PagesPaginationResponse : IPaginationResponse<PageEntity>
{
    [JsonProperty("pages")]
    public PaginationData<PageEntity> Items { get; set; }
}