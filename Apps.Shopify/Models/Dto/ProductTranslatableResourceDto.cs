using Apps.Shopify.Models.Request.TranslatableResource;

namespace Apps.Shopify.Models.Dto;

public class ProductTranslatableResourceDto
{
    public IEnumerable<IdentifiedContentRequest> ProductContentEntities { get; set; }
    public IEnumerable<IdentifiedContentRequest>? MetafieldsContentEntities { get; set; }
    public IEnumerable<IdentifiedContentRequest>? OptionsContentEntities { get; set; }
    public IEnumerable<IdentifiedContentRequest>? OptionValuesContentEntities { get; set; }
}