using Apps.Shopify.Models.Entities;

namespace Apps.Shopify.Models.Dto;

public class ProductContentDto
{
    public IEnumerable<ContentEntity> ProductContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? MetafieldsContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? OptionsContentEntities { get; set; }
    public IEnumerable<IdentifiedContentEntity>? OptionValuesContentEntities { get; set; }
}