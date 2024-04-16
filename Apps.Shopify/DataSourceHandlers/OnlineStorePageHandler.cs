using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class OnlineStorePageHandler : TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.ONLINE_STORE_PAGE;

    public OnlineStorePageHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}