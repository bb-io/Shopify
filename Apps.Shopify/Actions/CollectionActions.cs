using Apps.Shopify.Actions.Base;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Collection;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class CollectionActions : TranslatableResourceActions
{
    public CollectionActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get collection translation as HTML",
        Description = "Get content of a specific collection in HTML format")]
    public Task<FileResponse> GetCollectionContent(
        [ActionParameter] CollectionRequest input, [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetContentRequest getContentRequest)
        => GetResourceContent(input.CollectionId, locale.Locale, getContentRequest.Outdated ?? default);

    [Action("Update collection content from HTML",
        Description = "Update content of a specific collection from HTML file")]
    public Task UpdateCollectionContent([ActionParameter] CollectionRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.CollectionId, locale.Locale, file.File);
}