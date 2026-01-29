using Apps.Shopify.Models.Entities.Content;

namespace Apps.Shopify.Models.Response.Content;

public record SearchContentResponse(List<ContentItemEntity> Items);
