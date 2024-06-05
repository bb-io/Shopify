using Apps.Shopify.Actions.Base;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreTheme;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStoreThemeActions : TranslatableResourceActions
{
    public OnlineStoreThemeActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext, fileManagementClient)
    {
    }
    
    [Action("Get online store theme content as HTML",
        Description = "Get content of a specific online store theme in HTML format")]
    public Task<FileResponse> GetOnlineStoreThemeTranslationContent(
        [ActionParameter] OnlineStoreThemeRequest input, [ActionParameter] LocaleRequest locale)
        => GetResourceContent(input.OnlineStoreThemeId, locale.Locale);

    [Action("Update online store theme content from HTML",
        Description = "Update content of a specific online store theme from HTML file")]
    public Task UpdateOnlineStoreThemeContent([ActionParameter] OnlineStoreThemeRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(resourceRequest.OnlineStoreThemeId, locale.Locale, file.File);
}