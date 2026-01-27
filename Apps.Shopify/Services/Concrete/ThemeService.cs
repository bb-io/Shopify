using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.Net.Mime;

namespace Apps.Shopify.Services.Concrete;

public class ThemeService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var translatableContent = await _resourceService.GetTranslatableContent(
            input.ContentId, 
            input.Locale, 
            input.Outdated ?? default
        );

        if (input.AssetKeys != null)
        {
            translatableContent = translatableContent
                .Where(x => input.AssetKeys.Any(y => x.Key.StartsWith(y)))
                .ToList();
        }

        var html = ShopifyHtmlConverter.ToHtml(translatableContent, HtmlMetadataConstants.OnlineStoreThemeContent);
        return await fileManagementClient.UploadAsync(
            html, 
            MediaTypeNames.Text.Html, 
            $"{input.ContentId.GetShopifyItemId()}.html"
        );
    }

    public async Task Upload(UploadContentRequest input)
    {
        await _resourceService.UpdateResourceContent(input.ContentId, input.Locale, input.Content);
    }
}
