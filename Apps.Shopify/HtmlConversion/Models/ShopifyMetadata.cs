namespace Apps.Shopify.HtmlConversion.Models;

public record ShopifyMetadata
{
    public string? ContentType { get; set; }

    public string? MarketId { get; set; }
    
    public ShopifyMetadata Merge(ShopifyMetadata? overrides)
    {
        return new()
        {
            ContentType = overrides?.ContentType ?? ContentType,
            MarketId = overrides?.MarketId ?? MarketId,
        };
    }
}