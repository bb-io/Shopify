using Apps.Shopify.Models.Entities.Metaobject;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Metaobject;

public class MetaobjectsPaginationResponse : IPaginationResponse<MetaobjectEntity>
{
    [JsonProperty("metaobjects")]
    public PaginationData<MetaobjectEntity> Items { get; set; }
}