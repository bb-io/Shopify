namespace Apps.Shopify.Models.Entities;

public class OptionValueEntity
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public IEnumerable<ContentEntity> Translations { get; set; }
}