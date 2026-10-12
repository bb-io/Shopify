using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.PackingSlipTemplate;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.PackingSlipTemplate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Packing slip templates")]
public class PackingSlipTemplateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.PackingSlipTemplate;
    
    [Action("Search packing slip templates", Description = "Search packing slip templates with specific criteria")]
    public async Task<SearchPackingSlipTemplatesResponse> SearchPackingSlipTemplates(
        [ActionParameter] SearchPackingSlipTemplatesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new PackingSlipTemplateResponse(x)).ToList());
    }
    
    [Action("Download packing slip template", Description = "Download content of a specific packing slip template")]
    public async Task<FileResponse> DownloadPackingSlipTemplate(
        [ActionParameter] PackingSlipTemplateIdentifier packingSlipTemplateIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = packingSlipTemplateIdentifier.PackingSlipTemplateId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload packing slip template", Description = "Upload content of a specific packing slip template")]
    public Task UploadPackingSlipTemplate(
        [ActionParameter] UploadPackingSlipTemplateRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.PackingSlipTemplateId, locale.Locale, marketIdentifier.MarketId);
    }
}