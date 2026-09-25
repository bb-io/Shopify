using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Theme;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Theme;
using Blackbird.Applications.Sdk.Common.Invocation;
using System.Net.Mime;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Services.Models;

namespace Apps.Shopify.Services.Concrete;

public class ThemeService(InvocationContext invocationContext) : BaseContentService(invocationContext), IContentService
{
    protected override string ContentType => TranslatableResources.Theme;

    public async Task<FileRecord> Download(DownloadContentRequest input)
    {
        var translatableContent = await ResourceService.GetTranslatableContent(
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
            ContentType = ContentType.ToLower()
        };
        
        var html = ShopifyHtmlConverter.ToHtml(translatableContent, metadata);
        return new FileRecord(html, MediaTypeNames.Text.Html, input.ContentId.GetFileName(metadata.MarketId));
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        var response = await Client.Paginate<ThemeEntity, ThemesPaginationResponse>(GraphQlQueries.Themes, []);

        if (!string.IsNullOrEmpty(input.NameContains))
            response = response.Where(x => x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();

        var items = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Name)).ToList();
        return new(items);
    }

    public Task Upload(UploadContentServiceRequest input)
    {
        return UploadTranslatableResource(input);
    }
}
