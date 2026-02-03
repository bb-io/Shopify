using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Metafield;

public class MetafieldPaginationResponse : IPaginationResponse<MetafieldEntity>
{
    [JsonProperty("metafields")]
    public PaginationData<MetafieldEntity> Items { get; set; }
}