using Apps.Shopify.Invocables;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services;

public abstract class BaseContentService(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    public abstract string ContentType { get; }
    
    protected readonly TranslatableResourceService ResourceService = new(invocationContext);

    public virtual async Task Upload(UploadContentServiceRequest input)
    {
        await ResourceService.UpdateResourceContent(input.ContentId, input.Locale, input.HtmlContent, input.MarketId);
    }
}