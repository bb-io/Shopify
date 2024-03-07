using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class ProductPublicationsCreatedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "PRODUCT_PUBLICATIONS_CREATE";

    public ProductPublicationsCreatedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}