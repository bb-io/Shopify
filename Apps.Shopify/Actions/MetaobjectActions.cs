using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Metaobject;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Metaobject;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Metaobjects")]
public class MetaobjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.Metaobject;
    
    [Action("Search metaobjects", Description = "Search metaobjects with specific criteria")]
    public async Task<SearchMetaobjectsResponse> SearchMetaobjects([ActionParameter] SearchMetaobjectsRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new MetaobjectResponse(x)).ToList());
    }

    [Action("Download metaobject", Description = "Download content of a specific metaobject")]
    public async Task<FileResponse> DownloadMetaobject(
        [ActionParameter] MetaobjectIdentifier metaobjectIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = metaobjectIdentifier.MetaobjectId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload metaobject", Description = "Upload content of a specific metaobject")]
    public Task UploadMetaobject(
        [ActionParameter] UploadMetaobjectRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.MetaobjectId, locale.Locale, marketIdentifier.MarketId);
    }
}