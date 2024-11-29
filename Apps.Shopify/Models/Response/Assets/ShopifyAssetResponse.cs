using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Assets;

public class ListAssetResponse
{
    [JsonProperty("assets")]
    public List<AssetResponse> Assets { get; set; } = new();
}

public class AssetResponse
{
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;
}