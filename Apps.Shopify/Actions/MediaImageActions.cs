using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.MediaImage;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.MediaImage;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Media images")]
public class MediaImageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.MediaImage;
    
    [Action("Search media images", Description = "Search media images with specific criteria")]
    public async Task<SearchMediaImagesResponse> SearchMediaImages([ActionParameter] SearchMediaImagesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new MediaImageResponse(x)).ToList());
    }

    [Action("Download media image", Description = "Download content of a specific media image")]
    public async Task<FileResponse> DownloadMediaImage(
        [ActionParameter] MediaImageIdentifier imageIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = imageIdentifier.MediaImageId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload media image", Description = "Upload content of a specific media image")]
    public Task UploadMediaImage(
        [ActionParameter] UploadMediaImageRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.MediaImageId, locale.Locale, marketIdentifier.MarketId);
    }
}