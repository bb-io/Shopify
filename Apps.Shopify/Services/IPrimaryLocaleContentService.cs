using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Services.Models;

namespace Apps.Shopify.Services;

public interface IPrimaryLocaleContentService
{
    Task PreparePrimaryLocaleUpdate(PrimaryLocaleUpdateContext context, IReadOnlyList<IdentifiedContentRequest> patches);
}
