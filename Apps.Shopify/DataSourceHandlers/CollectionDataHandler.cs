using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class CollectionDataHandler : TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.COLLECTION;

    public CollectionDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}