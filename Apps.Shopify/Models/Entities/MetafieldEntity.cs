namespace Apps.Shopify.Models.Entities;

public class MetafieldEntity
{
    public string Id { get; set; }
    
    public string Key { get; set; }
    
    public string Namespace { get; set; }
    
    public string Value { get; set; }
    
    public string CompareDigest { get; set; }
}