using Apps.Shopify.Models.Entities;

namespace Apps.Shopify.Models.Response.Metafield;

public record SearchMetafieldsResponse(IEnumerable<MetafieldDefinitionEntity> Metafields);
