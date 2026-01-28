using Apps.Shopify.Models.Entities.Theme;

namespace Apps.Shopify.Models.Response.Theme;

public record SearchThemesResponse(IEnumerable<ThemeEntity> Themes);