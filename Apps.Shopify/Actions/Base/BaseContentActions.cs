using Apps.Shopify.Extensions;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Services;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions.Base;

public abstract class BaseContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : ShopifyInvocable(invocationContext)
{
    protected abstract string ContentType { get; }
    
    protected IContentService ContentService => new ContentServiceFactory(InvocationContext).GetContentService(ContentType);
    
    protected async Task<FileReference> DownloadContent(DownloadContentRequest request)
    {
        var fileRecord = await ContentService.Download(request);
        return await fileManagementClient.UploadFileRecord(fileRecord);
    }

    protected async Task UploadContent(FileReference file, string? contentId, string locale, string? marketId)
    {
        string htmlContent = await fileManagementClient.DownloadHtml(file);

        await ContentService.Upload(new UploadContentServiceRequest
        {
            HtmlContent = htmlContent,
            ContentId = contentId,
            Locale = locale,
            MarketId = marketId
        });
    }
}