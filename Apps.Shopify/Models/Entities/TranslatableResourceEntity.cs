namespace Apps.Shopify.Models.Entities;

public class TranslatableResourceEntity
{
    public string ResourceId { get; set; }
    
    public IEnumerable<ContentEntity> TranslatableContent { get; set; }

    public IEnumerable<ContentEntity> Translations { get; set; }
}