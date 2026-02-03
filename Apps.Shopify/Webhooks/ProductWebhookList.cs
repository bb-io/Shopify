using Apps.Shopify.Webhooks.Handlers;
using Apps.Shopify.Webhooks.Payloads;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Shopify.Webhooks;

[WebhookList("Product")]
public class ProductWebhookList : BaseWebhookList
{
    [Webhook("On product created", typeof(ProductCreatedHandler), Description = "On certain product created")]
    public Task<WebhookResponse<ProductPayload>> OnProductCreated(WebhookRequest request)
        => HandleWebhookRequest<ProductPayload>(request);

    [Webhook("On product deleted", typeof(ProductDeletedHandler), Description = "On certain product deleted")]
    public async Task<WebhookResponse<ProductIdPayload>> OnProductDeleted(WebhookRequest request)
    {
        var response = await HandleWebhookRequest<ProductIdPayload>(request);
        response.Result!.Id = $"gid://shopify/Product/{response.Result.Id}";

        return response;
    }

    [Webhook("On product updated", typeof(ProductUpdatedHandler), Description = "On certain product updated")]
    public Task<WebhookResponse<ProductPayload>> OnProductUpdated(WebhookRequest request)
        => HandleWebhookRequest<ProductPayload>(request);

    [Webhook("On product publications created", typeof(ProductPublicationsCreatedHandler),
        Description = "On certain product publications created")]
    public Task<WebhookResponse<ProductPayload>> OnProductPublished(WebhookRequest request)
        => HandleWebhookRequest<ProductPayload>(request);

    [Webhook("On product publications deleted", typeof(ProductPublicationsDeletedHandler),
        Description = "On certain product publications deleted")]
    public Task<WebhookResponse<ProductPayload>> OnProductUnpublished(WebhookRequest request)
        => HandleWebhookRequest<ProductPayload>(request);
}
