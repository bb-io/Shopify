using Apps.Shopify.Models.Entities.Theme;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Theme;

public class ThemesPaginationResponse : IPaginationResponse<ThemeEntity>
{
    [JsonProperty("themes")]
    public PaginationData<ThemeEntity> Items { get; set; }
}
