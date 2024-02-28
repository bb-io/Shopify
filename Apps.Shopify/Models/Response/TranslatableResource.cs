using Apps.Shopify.Models.Entities;

namespace Apps.Shopify.Models.Response;

public class TranslatableResource
{
    public IEnumerable<ContentEntity> TranslatableContent { get; set; }

    public IEnumerable<ContentEntity> Translations { get; set; }
}