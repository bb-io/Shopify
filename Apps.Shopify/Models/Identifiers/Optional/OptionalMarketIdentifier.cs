using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers.Optional;

public class OptionalMarketIdentifier
{
    [Display("Market ID"), DataSource(typeof(MarketDataHandler))]
    public string? MarketId { get; set; }
}