using Apps.Shopify.Models.Entities;

namespace Apps.Shopify.Models.Response.Theme;

public class ListThemesResponse
{
    public IEnumerable<ThemeEntity> Themes { get; set; }
}