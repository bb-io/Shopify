using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.DeliveryMethodDefinition;
using Apps.Shopify.Models.Request.StoreThemeJsonTemplate;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StoreThemeJsonTemplate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store theme JSON templates")]
public class StoreThemeJsonTemplateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StoreThemeJsonTemplate;
    
    [Action("Search store theme JSON templates", Description = "Search store theme JSON templates with specific criteria")]
    public async Task<SearchStoreThemeJsonTemplatesResponse> SearchStoreThemeJsonTemplate(
        [ActionParameter] SearchStoreThemeJsonTemplatesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StoreThemeJsonTemplateResponse(x)).ToList());
    }

    [Action("Download store theme JSON template", Description = "Download content of a specific store theme JSON template")]
    public async Task<FileResponse> DownloadStoreThemeJsonTemplate(
        [ActionParameter] StoreThemeJsonTemplateIdentifier templateIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = templateIdentifier.StoreThemeJsonTemplateId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store theme JSON template", Description = "Upload content of a specific store theme JSON template")]
    public Task UploadStoreThemeJsonTemplate(
        [ActionParameter] UploadStoreThemeJsonTemplateRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ThemeJsonTemplateId, locale.Locale, marketIdentifier.MarketId);
    }
}