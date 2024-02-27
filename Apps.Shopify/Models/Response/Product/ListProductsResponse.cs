using Apps.Shopify.Models.Entities;

namespace Apps.Shopify.Models.Response.Product;

public record ListProductsResponse(IEnumerable<ProductEntity> Products);