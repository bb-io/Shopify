namespace Apps.Shopify.Models.Entities;

public class MetafieldDefinitionEntity
{
    public string Id { get; set; }
    
    public string Key { get; set; }
    
    public string Name { get; set; }
    
    public DefinitionTypeEntity Type { get; set; }
    
    public string Namespace { get; set; }
}