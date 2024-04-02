using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class OnlineStoreBlogHandler : TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.ONLINE_STORE_BLOG;

    public OnlineStoreBlogHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}