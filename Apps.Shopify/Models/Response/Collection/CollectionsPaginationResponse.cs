using Apps.Shopify.Models.Entities.Collection;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Collection;

public class CollectionsPaginationResponse : IPaginationResponse<CollectionEntity>
{
    [JsonProperty("collections")]
    public PaginationData<CollectionEntity> Items { get; set; }
}
