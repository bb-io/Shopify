using Apps.Shopify.Actions.Base;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreArticle;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStoreArticleActions : TranslatableResourceActions
{
    public OnlineStoreArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get online store article translation as HTML",
        Description = "Get content of a specific online store article in HTML format")]
    public Task<FileResponse> GetOnlineStoreArticleTranslationContent(
        [ActionParameter] OnlineStoreArticleRequest input, [ActionParameter] LocaleRequest locale)
        => GetResourceContent(input.OnlineStoreArticleId, locale.Locale);

    [Action("Update online store article content from HTML",
        Description = "Update content of a specific online store article from HTML file")]
    public Task UpdateOnlineStoreArticleContent([ActionParameter] OnlineStoreArticleRequest resourceRequest,
        [ActionParameter] LocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.OnlineStoreArticleId, locale.Locale, file.File);
}