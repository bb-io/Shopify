using Apps.Shopify.Models.Entities.Collection;

namespace Apps.Shopify.Models.Response.Collection;

public record SearchCollectionsResponse(IEnumerable<CollectionEntity> Collections);
