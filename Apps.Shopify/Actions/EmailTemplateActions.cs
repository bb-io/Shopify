using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.EmailTemplate;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.EmailTemplate;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Email templates")]
public class EmailTemplateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.EmailTemplate;
    
    [Action("Search email templates", Description = "Search email templates with specific criteria")]
    public async Task<SearchEmailTemplatesResponse> SearchEmailTemplates([ActionParameter] SearchEmailTemplatesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new EmailTemplateResponse(x)).ToList());
    }

    [Action("Download email template", Description = "Download content of a specific email template")]
    public async Task<FileResponse> DownloadEmailTemplate(
        [ActionParameter] EmailTemplateIdentifier templateIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = templateIdentifier.EmailTemplateId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload email template", Description = "Upload content of a specific email template")]
    public Task UploadEmailTemplate(
        [ActionParameter] UploadEmailTemplateRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.EmailTemplateId, locale.Locale, marketIdentifier.MarketId);
    }
}