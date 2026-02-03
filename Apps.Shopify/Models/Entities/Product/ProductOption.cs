using Apps.Shopify.Models.Entities.Resource;

namespace Apps.Shopify.Models.Entities.Product;

public class ProductOption
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public IEnumerable<ProductOptionValue> OptionValues { get; set; }
    
    public IEnumerable<ContentEntity> Translations { get; set; }
}