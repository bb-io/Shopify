using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.DeliveryMethodDefinition;
using Apps.Shopify.Models.Response.DeliveryMethodDefinition;
using Apps.Shopify.Models.Response.Menu;
using Apps.Shopify.Services;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Delivery method definitions")]
public class DeliveryMethodDefinitionActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext);
    private readonly string _contentType = TranslatableResources.DeliveryMethodDefinition;
    
    [Action("Search delivery method definitions", Description = "Search delivery method definitions with specific criteria")]
    public async Task<SearchDeliveryMethodDefinitionsResponse> SearchDeliveryMethodDefinitions(
        [ActionParameter] SearchDeliveryMethodDefinitionsRequest input)
    {
        var service = _factory.GetContentService(_contentType);

        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await service.Search(searchInput);
        return new(items.Items.Select(x => new DeliveryMethodDefinitionResponse(x)).ToList());
    }

    [Action("Download delivery method definition", Description = "Download content of a specific delivery method definition")]
    public async Task<DownloadMenuResponse> DownloadDeliveryMethodDefinition(
        [ActionParameter] DeliveryMethodDefinitionIdentifier definitionIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var service = _factory.GetContentService(_contentType);
        var request = new DownloadContentRequest
        {
            ContentId = definitionIdentifier.DeliveryMethodDefinitionId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };
        
        var fileRecord = await service.Download(request);
        var file = await fileManagementClient.UploadFileRecord(fileRecord);
        return new(file);
    }
    
    [Action("Upload delivery method definition", Description = "Upload content of a specific delivery method definition")]
    public async Task UploadDeliveryMethodDefinition(
        [ActionParameter] UploadDeliveryMethodDefinitionRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var service = _factory.GetContentService(_contentType);
        string htmlContent = await fileManagementClient.DownloadHtml(input.File);
        
        var request = new UploadContentServiceRequest
        {
            HtmlContent = htmlContent,
            ContentId = input.DeliveryMethodDefinitionId,
            Locale = locale.Locale,
            MarketId = marketIdentifier.MarketId
        };

        await service.Upload(request);
    }
}