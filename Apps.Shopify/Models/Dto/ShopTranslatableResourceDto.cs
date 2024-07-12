using Apps.Shopify.Models.Request.TranslatableResource;

namespace Apps.Shopify.Models.Dto;

public class ShopTranslatableResourceDto
{
    public IEnumerable<IdentifiedContentRequest>? ThemesContentEntities { get; set; }
    public IEnumerable<IdentifiedContentRequest>? MenuContentEntities { get; set; }
    public IEnumerable<IdentifiedContentRequest>? ShopContentEntities { get; set; }
    public IEnumerable<IdentifiedContentRequest>? ShopPolicyContentEntities { get; set; }
}