using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class LocaleUpdatedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "LOCALES_UPDATE";

    public LocaleUpdatedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}