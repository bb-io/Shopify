using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class BlogDataHandler(InvocationContext invocationContext) : TranslatableResourceHandler(invocationContext)
{
    protected override TranslatableResource ResourceType => TranslatableResource.BLOG;
}