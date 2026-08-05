using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Metafield;

public class ProductMetafieldsPaginationResponse : IPaginationResponse<MetafieldEntity>
{
    [JsonProperty("product")]
    public required ProductMetafieldsNodeResponse Product { get; set; }

    public PaginationData<MetafieldEntity> Items 
    { 
        get => Product.Metafields; 
        set { } 
    }
}