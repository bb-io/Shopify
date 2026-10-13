using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.StorePolicy.Api;

public class StorePolicyOuterResponse
{
    [JsonProperty("shop")]
    public StorePoliciesResponse Store { get; set; } = null!;
}