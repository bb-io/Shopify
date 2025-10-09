using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Assets;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Online store themes")]
public class OnlineStoreThemeActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : TranslatableResourceActions(invocationContext, fileManagementClient)
{
    [Action("Download online store theme",
        Description = "Get content of a specific online store theme")]
    public async Task<FileResponse> GetOnlineStoreThemeTranslationContent(
        [ActionParameter] GetOnlineStoreThemeContentAsHtmlRequest input, 
        [ActionParameter] LocaleRequest locale,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var translatableContent = await GetTranslatableContent(input.OnlineStoreThemeId, locale.Locale, getContentRequest.Outdated ?? default);
        if (input.AssetKeys != null)
        {
            translatableContent = translatableContent
                .Where(x => input.AssetKeys.Any(y => x.Key.StartsWith(y)))
                .ToList();
        }
        
        var html = ShopifyHtmlConverter.ToHtml(translatableContent, HtmlContentTypes.OnlineStoreThemeContent);
        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"{input.OnlineStoreThemeId.GetShopifyItemId()}.html")
        };
    }

    [Action("Upload online store theme",
        Description = "Update content of a specific online store theme")]
    public Task UpdateOnlineStoreThemeContent(
        [ActionParameter, DataSource(typeof(OnlineStoreThemeDataSourceHandler)), Display("Online store theme ID")]
        string? onlineStoreThemeId,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
        => UpdateResourceContent(onlineStoreThemeId, locale.Locale, file.File);
}