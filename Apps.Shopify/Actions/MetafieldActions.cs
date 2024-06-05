using Apps.Shopify.Actions.Base;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Metafield;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class MetafieldActions : TranslatableResourceActions
{
    public MetafieldActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext, fileManagementClient)
    {
    }
    
    [Action("Get metafield content as HTML",
        Description = "Get content of a specific metafield in HTML format")]
    public Task<FileResponse> GetMetafieldContent([ActionParameter] MetafieldRequest resourceRequest,
        [ActionParameter] LocaleRequest locale)
        => GetResourceContent(resourceRequest.MetafieldId, locale.Locale);

    [Action("Update metafield content from HTML", Description = "Update content of a specific v from HTML file")]
    public Task UpdateMetafieldContent([ActionParameter] MetafieldRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.MetafieldId, locale.Locale, file.File);
}