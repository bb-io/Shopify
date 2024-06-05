using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class MetafieldDataSourceHandler : TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.METAFIELD;

    public MetafieldDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}