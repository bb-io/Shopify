using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.OnlineStore;
using Apps.Shopify.Models.Response.Locale;
using Apps.Shopify.Models.Response.Store;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using System.Net.Mime;

namespace Apps.Shopify.Actions;

[ActionList("Stores")]
public class StoreActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly TranslatableResourceService _translatableResourceService = new(invocationContext, fileManagementClient);

    [Action("Get store locales information", Description = "Get primary and additional store locales")]
    public async Task<StoreLocalesResponse> GetStoreLanguages()
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.Locales
        };
        var response = await Client.ExecuteWithErrorHandling<ShopLocalesResponse>(request);

        string primaryLocale = response.ShopLocales.First(x => x.Primary).Locale;
        IEnumerable<string> otherLocales = response.ShopLocales.Where(x => x.Primary is false).Select(x => x.Locale);
        return new StoreLocalesResponse(primaryLocale, otherLocales);
    }

    [Action("Download store resources", Description = "Download content of all store resource type items")]
    public async Task<DownloadStoreResourcesResponse> GetStoreResourcesContent(
        [ActionParameter] ResourceTypeIdentifier input,
        [ActionParameter] LocaleIdentifier locale, 
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest)
    {
        if (!Enum.TryParse(input.ResourceType, ignoreCase: true, out TranslatableResource resourceType))
            throw new PluginMisconfigurationException("Invalid resource type value specified. Please check the input");

        var resources = await _translatableResourceService.ListTranslatableResources(
            resourceType, 
            locale.Locale, 
            getContentRequest.Outdated ?? default
        );
        var contentEntities = resources.SelectMany(x =>
        {
            var content = x.GetTranslatableContent();
            return content.Select(y => new IdentifiedContentEntity(y)
            {
                Id = x.ResourceId
            });
        });
        
        var html = ShopifyHtmlConverter.ToHtml(contentEntities, TranslatableResource.STORE_RESOURCES.ToString().ToLower());
        var file = await fileManagementClient.UploadAsync(
            html, 
            MediaTypeNames.Text.Html, 
            $"{input.ResourceType}-{locale.Locale}.html"
        );
        return new(file);
    }

    [Action("Upload store resources", Description = "Upload content of all store resource type items")]
    public async Task UpdateStoreResourcesContent(
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] UploadStoreResourcesRequest input)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, input.Content);
        var content = ShopifyHtmlConverter.ToJson(html, locale.Locale).ToList();
        await _translatableResourceService.UpdateIdentifiedContent(content);
    }

    [Action("Download store content", Description = "Download content of the store")]
    public async Task<DownloadStoreContentResponse> GetStoreContent(
        [ActionParameter] DownloadStoreContentRequest input,
        [ActionParameter] LocaleIdentifier locale, 
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest)
    {
        if (NoneItemsIncluded(input))
            throw new PluginMisconfigurationException("You should include at least one content type. Please check your input and try again");

        var html = ShopifyHtmlConverter.StoreToHtml(new()
        {
            ThemesContentEntities = 
                input.IncludeThemes is true 
                ? await _translatableResourceService.ListIdentifiedTranslatableResources(
                    TranslatableResource.ONLINE_STORE_THEME, 
                    locale.Locale,
                    getContentRequest.Outdated ?? default
                )
                : null,
            MenuContentEntities = 
                input.IncludeMenu is true
                ? await _translatableResourceService.ListIdentifiedTranslatableResources(
                    TranslatableResource.ONLINE_STORE_MENU, 
                    locale.Locale,
                    getContentRequest.Outdated ?? default
                )
                : null,
            ShopContentEntities = 
                input.IncludeShop is true
                ? await _translatableResourceService.ListIdentifiedTranslatableResources(
                    TranslatableResource.SHOP, 
                    locale.Locale,
                    getContentRequest.Outdated ?? default
                )
                : null,
            ShopPolicyContentEntities = 
                input.IncludeShopPolicy is true
                ? await _translatableResourceService.ListIdentifiedTranslatableResources(
                    TranslatableResource.SHOP_POLICY, 
                    locale.Locale,
                    getContentRequest.Outdated ?? default
                )
                : null,
        });

        var file = await fileManagementClient.UploadAsync(
            html,
            MediaTypeNames.Text.Html,
            $"Shop-{locale.Locale}.html"
        );
        return new(file);
    }

    [Action("Upload store content", Description = "Upload content of the store")]
    public async Task UpdateStoreContent(
        [ActionParameter] LocaleIdentifier locale, 
        [ActionParameter] UploadStoreContentRequest input)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, input.Content);
        var content = ShopifyHtmlConverter.StoreToJson(html, locale.Locale);

        await _translatableResourceService.UpdateIdentifiedContent(content.ThemesContentEntities?.ToList());
        await _translatableResourceService.UpdateIdentifiedContent(content.MenuContentEntities?.ToList());
        await _translatableResourceService.UpdateIdentifiedContent(content.ShopContentEntities?.ToList());
        await _translatableResourceService.UpdateIdentifiedContent(content.ShopPolicyContentEntities?.ToList());
    }

    private static bool NoneItemsIncluded(DownloadStoreContentRequest input)
    {
        return 
            input.IncludeThemes is not true && 
            input.IncludeMenu is not true && 
            input.IncludeShop is not true &&
            input.IncludeShopPolicy is not true;
    }
}