namespace Apps.Shopify.HtmlConversion.Models;

public record ShopifyMetadata
{
    public string? ContentType { get; set; }

    public string? MarketId { get; set; }
    
    public static implicit operator ShopifyMetadata(string contentType) => new() { ContentType = contentType };
}