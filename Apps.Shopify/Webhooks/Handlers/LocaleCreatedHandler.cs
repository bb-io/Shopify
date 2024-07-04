using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class LocaleCreatedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "LOCALES_CREATE";

    public LocaleCreatedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}