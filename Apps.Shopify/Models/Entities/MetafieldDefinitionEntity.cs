using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities;

public class MetafieldDefinitionEntity
{
    [Display("ID")]
    public string Id { get; set; }
    
    [Display("Key")]    
    public string Key { get; set; }

    [Display("Name")]
    public string Name { get; set; }

    [Display("Type")]
    public DefinitionTypeEntity Type { get; set; }

    [Display("Namespace")]
    public string Namespace { get; set; }
}