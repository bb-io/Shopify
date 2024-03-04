using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class ProductDeletedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "PRODUCTS_DELETE";

    public ProductDeletedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}