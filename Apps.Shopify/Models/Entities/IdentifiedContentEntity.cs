namespace Apps.Shopify.Models.Entities;

public class IdentifiedContentEntity : ContentEntity
{
    public string Id { get; set; }

    public IdentifiedContentEntity()
    {
    }

    public IdentifiedContentEntity(ContentEntity contentEntity)
    {
        Key = contentEntity.Key;
        Value = contentEntity.Value;
        Digest = contentEntity.Digest;
    }
}