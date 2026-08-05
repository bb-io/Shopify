using System.Text.Json.Serialization;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Response.Pagination;

namespace Apps.Shopify.Models.Response.Metafield;

public class ProductMetafieldsNodeResponse
{
    [JsonPropertyName("metafields")]
    public PaginationData<MetafieldEntity> Metafields { get; set; }
}