using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class OnlineStorePageHandler : TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.PAGE;

    public OnlineStorePageHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}