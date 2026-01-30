using Apps.Shopify.Webhooks.Handlers;
using Apps.Shopify.Webhooks.Payloads;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Shopify.Webhooks;

[WebhookList("Locales")]
public class LocaleWebhookList : BaseWebhookList
{
    [Webhook("On locale created", typeof(LocaleCreatedHandler), Description = "On a new locale added to the store")]
    public Task<WebhookResponse<LocalePayload>> OnLocaleCreated(WebhookRequest request)
        => HandleWebhookRequest<LocalePayload>(request);

    [Webhook("On locale updated", typeof(LocaleUpdatedHandler), Description = "On specific locale published or unpublished")]
    public Task<WebhookResponse<LocalePayload>> OnLocaleUpdated(WebhookRequest request)
        => HandleWebhookRequest<LocalePayload>(request);
}