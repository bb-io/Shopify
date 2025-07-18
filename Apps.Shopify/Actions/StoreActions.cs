using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStore;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions;

[ActionList]
public class StoreActions : TranslatableResourceActions
{
    public StoreActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get store locales information", Description = "Get primary and other locales")]
    public async Task<StoreLocalesResponse> GetStoreLanguages()
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.Locales
        };
        var response = await Client.ExecuteWithErrorHandling<ShopLocalesResponse>(request);

        return new StoreLocalesResponse
        {
            Primary = response.ShopLocales.First(x => x.Primary).Locale,
            OtherLocales = response.ShopLocales.Where(x => x.Primary is false).Select(x => x.Locale)
        };
    }

    [Action("Get store resources content as HTML",
        Description = "Get content of all store resource type items in HTML format")]
    public async Task<FileResponse> GetStoreResourcesContent([ActionParameter] ResourceTypeRequest input,
        [ActionParameter] LocaleRequest locale, [ActionParameter] GetContentRequest getContentRequest)

    {
        if (!Enum.TryParse(input.ResourceType, ignoreCase: true, out TranslatableResource resourceType))
            throw new PluginMisconfigurationException("Invalid resource type value specified. Please check the input");

        var resources =
            await ListTranslatableResources(resourceType, locale.Locale, getContentRequest.Outdated ?? default);
        var contentEntities = resources.SelectMany(x =>
        {
            var content = x.GetTranslatableContent();
            return content.Select(y => new IdentifiedContentEntity(y)
            {
                Id = x.ResourceId
            });
        });
        
        var html = ShopifyHtmlConverter.ToHtml(contentEntities, HtmlContentTypes.StoreResourcesContent);
        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"{input.ResourceType}-{locale.Locale}.html")
        };
    }

    [Action("Update store resources content from HTML",
        Description = "Update content of all store resource type items from an HTML file")]
    public async Task UpdateStoreResourcesContent([ActionParameter] LocaleRequest locale, FileRequest file)

    {
        var html = await GetHtmlFromFile(file.File);
        var content = ShopifyHtmlConverter.ToJson(html, locale.Locale).ToList();
        await UpdateIdentifiedContent(content);
    }


    [Action("Get store content as HTML",
        Description = "Get content of the store in HTML format")]
    public async Task<FileResponse> GetStoreContent([ActionParameter] StoreContentRequest input,
        [ActionParameter] LocaleRequest locale, [ActionParameter] GetContentRequest getContentRequest)

    {
        if (NoneItemsIncluded(input))
            throw new PluginMisconfigurationException("You should include at least one content type. Please check your input and try again");

        var html = ShopifyHtmlConverter.StoreToHtml(new()
        {
            ThemesContentEntities = input.IncludeThemes is true
                ? await ListIdentifiedTranslatableResources(TranslatableResource.ONLINE_STORE_THEME, locale.Locale,
                    getContentRequest.Outdated ?? default)
                : null,
            MenuContentEntities = input.IncludeMenu is true
                ? await ListIdentifiedTranslatableResources(TranslatableResource.ONLINE_STORE_MENU, locale.Locale,
                    getContentRequest.Outdated ?? default)
                : null,
            ShopContentEntities = input.IncludeShop is true
                ? await ListIdentifiedTranslatableResources(TranslatableResource.SHOP, locale.Locale,
                    getContentRequest.Outdated ?? default)
                : null,
            ShopPolicyContentEntities = input.IncludeShopPolicy is true
                ? await ListIdentifiedTranslatableResources(TranslatableResource.SHOP_POLICY, locale.Locale,
                    getContentRequest.Outdated ?? default)
                : null,
        });

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"Shop-{locale.Locale}.html")
        };
    }

    [Action("Update store content from HTML",
        Description = "Update content of the store from an HTML file")]
    public async Task UpdateStoreContent([ActionParameter] LocaleRequest locale, FileRequest file)

    {
        var html = await GetHtmlFromFile(file.File);
        var content = ShopifyHtmlConverter.StoreToJson(html, locale.Locale);

        await UpdateIdentifiedContent(content.ThemesContentEntities?.ToList());
        await UpdateIdentifiedContent(content.MenuContentEntities?.ToList());
        await UpdateIdentifiedContent(content.ShopContentEntities?.ToList());
        await UpdateIdentifiedContent(content.ShopPolicyContentEntities?.ToList());
    }

    private bool NoneItemsIncluded(StoreContentRequest input)
    {
        return input.IncludeThemes is not true && input.IncludeMenu is not true && input.IncludeShop is not true &&
               input.IncludeShopPolicy is not true;
    }
}