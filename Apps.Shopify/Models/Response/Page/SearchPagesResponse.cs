using Apps.Shopify.Models.Entities.Page;

namespace Apps.Shopify.Models.Response.Page;

public record SearchPagesResponse(IEnumerable<PageEntity> Pages);