using Newtonsoft.Json;

namespace Apps.Shopify.Models.Entities.Menu;

public class MenuEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("handle")]
    public string Handle { get; set; } = string.Empty;

    [JsonProperty("items")]
    public List<MenuItemEntity> Items { get; set; } = [];
}