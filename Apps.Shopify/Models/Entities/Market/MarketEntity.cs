using Newtonsoft.Json;

namespace Apps.Shopify.Models.Entities.Market;

public class MarketEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}