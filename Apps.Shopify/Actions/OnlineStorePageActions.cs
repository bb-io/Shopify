using Apps.Shopify.Actions.Base;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStorePage;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStorePageActions : TranslatableResourceActions
{
    public OnlineStorePageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get online store page content as HTML",
        Description = "Get content of a specific online store page in HTML format")]
    public Task<FileResponse> GetOnlineStorePageTranslationContent(
        [ActionParameter] OnlineStorePageRequest input, [ActionParameter] LocaleRequest locale)
        => GetResourceContent(input.OnlineStorePageId, locale.Locale);

    [Action("Update online store page content from HTML",
        Description = "Update content of a specific online store page from HTML file")]
    public Task UpdateOnlineStorePageContent([ActionParameter] OnlineStorePageRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.OnlineStorePageId, locale.Locale, file.File);
}