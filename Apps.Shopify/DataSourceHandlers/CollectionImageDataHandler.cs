using Apps.Shopify.DataSourceHandlers.TranslatableResourceBase;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class CollectionImageDataHandler(InvocationContext invocationContext) 
    : TranslatableResourceDataHandler(invocationContext)
{
    protected override TranslatableResource ResourceType => TranslatableResource.COLLECTION_IMAGE;
}