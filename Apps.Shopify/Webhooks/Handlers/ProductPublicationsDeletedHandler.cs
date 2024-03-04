using Apps.Shopify.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Webhooks.Handlers;

public class ProductPublicationsDeletedHandler : ShopifyWebhookHandler
{
    protected override string Topic => "PRODUCT_PUBLICATIONS_DELETE";

    public ProductPublicationsDeletedHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}