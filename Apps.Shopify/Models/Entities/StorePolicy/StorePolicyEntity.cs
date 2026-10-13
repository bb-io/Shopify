using Newtonsoft.Json;

namespace Apps.Shopify.Models.Entities.StorePolicy;

public class StorePolicyEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
}