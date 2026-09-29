using Apps.Shopify.Constants;
using Apps.Shopify.DataSourceHandlers.TranslatableResourceBase;
using Apps.Shopify.Models.Request.Content;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ContentDataHandler : TranslatableResourceDataHandler
{
    private readonly ContentTypeIdentifier _contentType;

    public ContentDataHandler(InvocationContext context, [ActionParameter] ContentTypeIdentifier contentType) : base(context)
    {
        if (string.IsNullOrEmpty(contentType.ContentType))
            throw new PluginMisconfigurationException("Please specify content type first.");
        
        _contentType = contentType;
    }

    protected override TranslatableResource ResourceType => TranslatableResources.GetApiType(_contentType.ContentType);
}
