using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities;

public class MetafieldEntity
{
    [Display("Metafield ID")]
    public string Id { get; set; }
    
    public string Key { get; set; }
    
    public string Namespace { get; set; }
    
    public string Value { get; set; }
    
    [Display("Compare digest")]
    public string CompareDigest { get; set; }
}