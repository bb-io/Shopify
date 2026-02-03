using Apps.Shopify.Models.Entities.Resource;

namespace Apps.Shopify.Models.Entities.Product;

public class ProductOptionValue
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public IEnumerable<ContentEntity> Translations { get; set; }
}