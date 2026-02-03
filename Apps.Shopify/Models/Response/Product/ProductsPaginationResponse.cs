using Apps.Shopify.Models.Entities.Product;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Product;

public class ProductsPaginationResponse : IPaginationResponse<ProductEntity>
{
    [JsonProperty("Products")]
    public PaginationData<ProductEntity> Items { get; set; }
}