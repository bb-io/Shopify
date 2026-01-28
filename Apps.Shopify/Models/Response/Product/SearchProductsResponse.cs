namespace Apps.Shopify.Models.Response.Product;

public record SearchProductsResponse(IEnumerable<GetProductResponse> Products);