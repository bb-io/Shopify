using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Collection;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
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

    [Action("Download collection",
        Description = "Get content of a specific collection")]
    public Task<FileResponse> GetCollectionContent(
        [ActionParameter] CollectionRequest input, [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetContentRequest getContentRequest)
        => GetResourceContent(input.CollectionId, locale.Locale, getContentRequest.Outdated ?? default, HtmlContentTypes.Collection);

    [Action("Upload collection",
        Description = "Upload content of a specific collection")]
    public Task UpdateCollectionContent(
        [ActionParameter, DataSource(typeof(CollectionDataHandler)), Display("Collection ID")]
        string? collectionId,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(collectionId, locale.Locale, file.File);
}