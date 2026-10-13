using System.Net.Mime;
using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion.PackingSlipTemplate;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Services.Base;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class PackingSlipTemplateService(InvocationContext invocationContext) 
    : BaseTranslatableResourceContentService(invocationContext)
{
    protected override string ContentType => TranslatableResources.PackingSlipTemplate;
    
    public override async Task<FileRecord> Download(DownloadContentRequest input)
    {
        var translatableContent = await ResourceService.GetTranslatableContent(
            input.ContentId,
            input.Locale,
            input.Outdated ?? false,
            input.MarketId);
        
        var htmlStream = PackingSlipTemplateHtmlConverter.ToHtml(translatableContent, CreateMetadata(input.MarketId));
        return new FileRecord(htmlStream, MediaTypeNames.Text.Html, input.ContentId.GetFileName(input.MarketId));
    }

    public override Task Upload(UploadContentServiceRequest input)
    {
        var items = PackingSlipTemplateHtmlConverter.ToJson(input.HtmlContent, input.Locale, input.MarketId); 
        return ResourceService.UpdateIdentifiedContent(items, input.ContentId, input.MarketId);
    }
}