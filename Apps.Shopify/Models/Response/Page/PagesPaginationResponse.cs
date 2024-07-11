using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Page;

public class PagesPaginationResponse : IRestPaginationResponse<OnlineStorePageEntity>
{
    [JsonProperty("pages")]
    public IEnumerable<OnlineStorePageEntity> Items { get; set; }
}