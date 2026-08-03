using Apps.Shopify.Constants;

namespace Apps.Shopify.HtmlConversion.Models;

public record ShopifyMetadata
{
    public string? ContentType { get; set; }

    public string? MarketId { get; set; }
    
    public ShopifyMetadata Merge(ShopifyMetadata? overrides)
    {
        string? market = overrides?.MarketId ?? MarketId;

        return new()
        {
            ContentType = overrides?.ContentType ?? ContentType,
            MarketId = market == MarketConstants.Global ? null : market
        };
    }
}