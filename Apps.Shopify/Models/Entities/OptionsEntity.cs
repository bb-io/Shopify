namespace Apps.Shopify.Models.Entities;

public class OptionsEntity
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public IEnumerable<OptionValueEntity> OptionValues { get; set; }
    
    public IEnumerable<ContentEntity> Translations { get; set; }
}