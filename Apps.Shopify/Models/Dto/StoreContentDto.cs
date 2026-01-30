using Apps.Shopify.Models.Entities.Resource;

namespace Apps.Shopify.Models.Dto;

public class StoreContentDto
{
    public IEnumerable<IdentifiedContentEntity>? ThemesContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? MenuContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? ShopContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? ShopPolicyContentEntities { get; set; }
}