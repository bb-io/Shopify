using Apps.Shopify.Services;
using Apps.Shopify.Invocables;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.Services.Models;

namespace Apps.Shopify.Actions;

[ActionList("Content")]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext);

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload content", Description = "Upload content of a specific content type from a file")]
    public async Task UploadContent([ActionParameter] UploadContentRequest input)
    {
        string htmlContent = await fileManagementClient.DownloadHtml(input.Content);
        string contentType = 
            input.ContentType ?? 
            ShopifyHtmlConverter.ExtractContentTypeFromHtml(htmlContent) ??
            throw new PluginMisconfigurationException("Content type is missing. Provide it in the input or include it in the file");

        var service = _factory.GetContentService(contentType);
        var request = new UploadContentServiceRequest
        {
            HtmlContent = htmlContent,
            ContentId = input.ContentId,
            Locale = input.Locale,
            MarketId = input.MarketId
        };
        
        await service.Upload(request);
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download content", Description = "Download content of a specific content type")]
    public async Task<DownloadContentResponse> DownloadContent(
        [ActionParameter] ContentTypeIdentifier contentType,
        [ActionParameter] DownloadContentRequest input)
    {
        var service = _factory.GetContentService(contentType.ContentType);
        var fileRecord = await service.Download(input);
        
        var file = await fileManagementClient.UploadFileRecord(fileRecord);
        return new(file);
    }

    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search through different content types with specific criteria")]
    public async Task<SearchContentResponse> SearchContent([ActionParameter] SearchContentRequest input)
    {
        input.Validate();

        input.ContentTypes ??= TranslatableResources.SupportedContentTypes;
        var services = _factory.GetContentServices(input.ContentTypes);
        return await services.ExecuteMany(input);
    }
}