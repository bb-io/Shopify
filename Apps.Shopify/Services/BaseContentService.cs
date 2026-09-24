using Apps.Shopify.Invocables;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services;

public abstract class BaseContentService(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    public abstract string ContentType { get; }
}