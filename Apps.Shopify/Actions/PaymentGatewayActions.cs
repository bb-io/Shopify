using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.PaymentGateway;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.PaymentGateway;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Payment gateways")]
public class PaymentGatewayActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.PaymentGateway;
    
    [Action("Search payment gateways", Description = "Search payment gateways with specific criteria")]
    public async Task<SearchPaymentGatewaysResponse> SearchPaymentGateways(
        [ActionParameter] SearchPaymentGatewaysRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new PaymentGatewayResponse(x)).ToList());
    }

    [Action("Download payment gateway", Description = "Download content of a specific payment gateway")]
    public async Task<FileResponse> DownloadPaymentGateway(
        [ActionParameter] PaymentGatewayIdentifier paymentGatewayIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = paymentGatewayIdentifier.PaymentGatewayId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload payment gateway", Description = "Upload content of a specific payment gateway")]
    public Task UploadPaymentGateway(
        [ActionParameter] UploadPaymentGatewayRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.PaymentGatewayId, locale.Locale, marketIdentifier.MarketId);
    }
}