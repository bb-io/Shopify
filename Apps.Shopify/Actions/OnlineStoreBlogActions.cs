using Apps.Shopify.Actions.Base;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStoreBlogActions : TranslatableResourceActions
{
    public OnlineStoreBlogActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get online store blog content as HTML",
        Description = "Get content of a specific online store blog in HTML format")]
    public Task<FileResponse> GetOnlineStoreBlogTranslationContent(
        [ActionParameter] OnlineStoreBlogRequest input, [ActionParameter] LocaleRequest locale)
        => GetResourceContent(input.OnlineStoreBlogId, locale.Locale);

    [Action("Update online store blog content from HTML",
        Description = "Update content of a specific online store blog from HTML file")]
    public Task UpdateOnlineStoreBlogContent([ActionParameter] OnlineStoreBlogRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.OnlineStoreBlogId, locale.Locale, file.File);
}