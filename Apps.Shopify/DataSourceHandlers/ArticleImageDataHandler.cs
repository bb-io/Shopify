using Apps.Shopify.DataSourceHandlers.TranslatableResourceBase;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ArticleImageDataHandler(InvocationContext invocationContext) 
    : TranslatableResourceDataHandler(invocationContext)
{
    protected override TranslatableResource ResourceType => TranslatableResource.ARTICLE_IMAGE;
}