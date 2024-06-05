using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class OnlineStoreThemeDataSourceHandler : TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.ONLINE_STORE_THEME;

    public OnlineStoreThemeDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}