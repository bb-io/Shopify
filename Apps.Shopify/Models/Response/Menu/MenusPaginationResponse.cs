using Apps.Shopify.Models.Entities.Menu;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Menu;

public class MenusPaginationResponse : IPaginationResponse<MenuEntity>
{
    [JsonProperty("menus")]
    public PaginationData<MenuEntity> Items { get; set; }
}