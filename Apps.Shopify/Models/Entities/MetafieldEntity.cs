using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities;

public class MetafieldEntity
{
    [Display("Metafield ID")]
    public string Id { get; set; }

    [Display("Key")]
    public string Key { get; set; }

    [Display("Namespace")]
    public string Namespace { get; set; }

    [Display("Value")]
    public string Value { get; set; }
    
    [Display("Compare digest")]
    public string CompareDigest { get; set; }
}