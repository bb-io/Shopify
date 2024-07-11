﻿using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
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
            Primary = response.ShopLocales.First(x => x.Primary),
            OtherLocales = response.ShopLocales.Where(x => x.Primary is false)
        };
    }

    [Action("Get store resources content as HTML",
        Description = "Get content of all store resource type items in HTML format")]
    public async Task<FileResponse> GetStoreResourcesContent([ActionParameter] ResourceTypeRequest input,
        [ActionParameter] LocaleRequest locale, [ActionParameter] GetContentRequest getContentRequest)

    {
        if (!Enum.TryParse(input.ResourceType, ignoreCase: true, out TranslatableResource resourceType))
            throw new("Invalid resource type value specified");

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
        var html = ShopifyHtmlConverter.ToHtml(contentEntities);

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
        var fileStream = await FileManagementClient.DownloadAsync(file.File);
        var content = ShopifyHtmlConverter.ToJsonIdentified(fileStream, locale.Locale).ToList();
        await UpdateIdentifiedContent(content);
    }
}