using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.CollectionImage;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.CollectionImage;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Collection images")]
public class CollectionImageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.CollectionImage;
    
    [Action("Search collection images", Description = "Search collection images with specific criteria")]
    public async Task<SearchCollectionImagesResponse> SearchCollectionImages(
        [ActionParameter] SearchCollectionImagesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new CollectionImageResponse(x)).ToList());
    }

    [Action("Download collection image", Description = "Download content of a specific collection image")]
    public async Task<FileResponse> DownloadCollectionImage(
        [ActionParameter] CollectionImageIdentifier collectionImageIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = collectionImageIdentifier.CollectionImageId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload collection image", Description = "Upload content of a specific collection image")]
    public Task UploadCollectionImage(
        [ActionParameter] UploadCollectionImageRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.CollectionImageId, locale.Locale, marketIdentifier.MarketId);
    }
}