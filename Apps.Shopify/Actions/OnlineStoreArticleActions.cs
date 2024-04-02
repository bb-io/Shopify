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

    [Action("Get online store article content as HTML",
        Description = "Get content of a specific online store article in HTML format")]
    public Task<FileResponse> GetOnlineStoreArticleContent([ActionParameter] OnlineStoreArticleRequest input)
        => GetResourceContent(input.OnlineStoreArticleId);

    [Action("Get online store article translation content as HTML",
        Description = "Get content of a specific online store article translation in HTML format")]
    public Task<FileResponse> GetOnlineStoreArticleTranslationContent(
        [ActionParameter] OnlineStoreArticleRequest input, [ActionParameter] LocaleRequest locale)
        => GetResourceTranslationContent(input.OnlineStoreArticleId, locale.Locale);

    [Action("Update online store article content from HTML",
        Description = "Update content of a specific online store article from HTML file")]
    public Task UpdateOnlineStoreArticleContent([ActionParameter] OnlineStoreArticleRequest resourceRequest,
        [ActionParameter] LocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.OnlineStoreArticleId, locale.Locale, file.File);
}