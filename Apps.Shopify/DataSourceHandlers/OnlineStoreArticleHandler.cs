using Apps.Shopify.DataSourceHandlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class OnlineStoreArticleHandler: TranslatableResourceHandler
{
    protected override TranslatableResource ResourceType => TranslatableResource.ARTICLE;

    public OnlineStoreArticleHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}