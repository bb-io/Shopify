using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class ProductUpdatedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "PRODUCTS_UPDATE";

    public ProductUpdatedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}