using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Metafield;

public class MetafieldDefinitionPaginationResponse : IPaginationResponse<MetafieldDefinitionEntity>
{
    [JsonProperty("metafieldDefinitions")]
    public PaginationData<MetafieldDefinitionEntity> Items { get; set; }
}