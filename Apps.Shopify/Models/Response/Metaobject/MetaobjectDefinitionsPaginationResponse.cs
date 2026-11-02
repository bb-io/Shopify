using Apps.Shopify.Models.Entities.Metaobject;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Metaobject;

public class MetaobjectDefinitionsPaginationResponse : IPaginationResponse<MetaobjectDefinitionEntity>
{
    [JsonProperty("metaobjectDefinitions")]
    public PaginationData<MetaobjectDefinitionEntity> Items { get; set; }
}