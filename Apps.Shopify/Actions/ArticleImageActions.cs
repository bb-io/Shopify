using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.ArticleImage;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.ArticleImage;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Article images")]
public class ArticleImageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.ArticleImage;
    
    [Action("Search article images", Description = "Search article images with specific criteria")]
    public async Task<SearchArticleImagesResponse> SearchArticleImages(
        [ActionParameter] SearchArticleImagesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new ArticleImageResponse(x)).ToList());
    }

    [Action("Download article image", Description = "Download content of a specific article image")]
    public async Task<FileResponse> DownloadArticleImage(
        [ActionParameter] ArticleImageIdentifier articleImageIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = articleImageIdentifier.ArticleImageId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload article image", Description = "Upload content of a specific article image")]
    public Task UploadArticleImage(
        [ActionParameter] UploadArticleImageRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ArticleImageId, locale.Locale, marketIdentifier.MarketId);
    }
}