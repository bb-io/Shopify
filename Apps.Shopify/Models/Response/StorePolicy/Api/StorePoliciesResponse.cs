using Apps.Shopify.Models.Entities.StorePolicy;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.StorePolicy.Api;

public class StorePoliciesResponse
{
    [JsonProperty("shopPolicies")]
    public List<StorePolicyEntity> StorePolicies { get; set; } = [];
}