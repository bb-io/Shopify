using Apps.Shopify.Models.Entities.Metafield;

namespace Apps.Shopify.Models.Response.Metafield;

public record SearchMetafieldsResponse(IEnumerable<MetafieldDefinitionEntity> Metafields);
