using Apps.Shopify.Models.Entities.Menu;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Menu;

public class MenuApiResponse
{
    [JsonProperty("menu")]
    public MenuEntity Menu { get; set; } = null!;
}