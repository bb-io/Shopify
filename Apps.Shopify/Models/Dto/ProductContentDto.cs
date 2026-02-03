using Apps.Shopify.Models.Entities.Resource;

namespace Apps.Shopify.Models.Dto;

public class ProductContentDto
{
    public IEnumerable<IdentifiedContentEntity> ProductContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? MetafieldsContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? OptionsContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? OptionValuesContentEntities { get; set; }
}