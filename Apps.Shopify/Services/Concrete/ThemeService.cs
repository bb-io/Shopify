using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Theme;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Theme;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.Net.Mime;
using Apps.Shopify.HtmlConversion.Models;

namespace Apps.Shopify.Services.Concrete;

public class ThemeService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);
    private readonly string _contentType = TranslatableResources.Theme;

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var translatableContent = await _resourceService.GetTranslatableContent(
            input.ContentId, 
            input.Locale, 
            input.Outdated ?? false,
            input.MarketId);

        if (input.AssetKeys != null)
        {
            translatableContent = translatableContent
                .Where(x => input.AssetKeys.Any(y => x.Key.StartsWith(y)))
                .ToList();
        }

        var metadata = new ShopifyMetadata
        {
            MarketId = input.MarketId,
            ContentType = _contentType.ToLower()
        };
        
        var html = ShopifyHtmlConverter.ToHtml(translatableContent, metadata);
        return await fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, input.ContentId.GetFileName(metadata.MarketId));
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        var response = await Client.Paginate<ThemeEntity, ThemesPaginationResponse>(GraphQlQueries.Themes, []);

        if (!string.IsNullOrEmpty(input.NameContains))
            response = response.Where(x => x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();

        var items = response.Select(x => new ContentItemEntity(x.Id, _contentType, x.Name)).ToList();
        return new(items);
    }

    public async Task Upload(UploadContentRequest input)
    {
        await _resourceService.UpdateResourceContent(input.ContentId, input.Locale, input.Content, input.MarketId);
    }
}
