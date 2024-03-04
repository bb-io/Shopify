using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class ProductCreatedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "PRODUCTS_CREATE";

    public ProductCreatedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}