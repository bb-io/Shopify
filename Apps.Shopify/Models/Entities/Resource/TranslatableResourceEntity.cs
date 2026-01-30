namespace Apps.Shopify.Models.Entities.Resource;

public class TranslatableResourceEntity
{
    public string ResourceId { get; set; }

    public IEnumerable<ContentEntity> TranslatableContent { get; set; }

    public IEnumerable<ContentEntity> Translations { get; set; }

    public IEnumerable<ContentEntity> GetTranslatableContent()
    {
        return Translations.Any()
            ? Translations
            : TranslatableContent;
    }
}