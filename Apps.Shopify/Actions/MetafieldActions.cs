using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Api;
using Apps.Shopify.Api.Rest;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Metafield;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Metafield;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using GraphQL;
using RestSharp;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Shopify.Actions;

[ActionList]
public class MetafieldActions : TranslatableResourceActions
{
    public MetafieldActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext, fileManagementClient)
    {
    }

    [Action("Get metafield content as HTML",
        Description = "Get metafield content of a specific product in HTML format")]
    public async Task<FileResponse> GetMetafieldContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleRequest locale, [ActionParameter] GetContentRequest getContentRequest)
    {
        var productMetaFields = await GetProductMetafields(resourceRequest.ProductId);
        var metaFields = await ListTranslatableResources(TranslatableResource.METAFIELD, locale.Locale,
            getContentRequest.Outdated ?? default);

        var resources = metaFields
            .Where(x => productMetaFields.Any(y => x.ResourceId == y.Id))
            .ToArray();

        var contents = resources.All(x => !x.Translations.Any())
            ? resources.Select(x => (x.ResourceId, x.TranslatableContent.FirstOrDefault())).ToArray()
            : resources.Select(x => (x.ResourceId, x.Translations.FirstOrDefault())).ToArray();

        var html = ShopifyHtmlConverter.MetaFieldsToHtml(contents.Where(x => x.Item2 is not null), HtmlContentTypes.MetafieldContent);
        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"{resourceRequest.ProductId.GetShopifyItemId()}-metafields.html")
        };
    }

    [Action("Update metafield content from HTML",
        Description = "Update metafield content of a specific product from HTML file")]
    public async Task UpdateMetaFieldContent([ActionParameter] NonPrimaryLocaleRequest locale,
        [ActionParameter] FileRequest file)
    {
        var html = await GetHtmlFromFile(file.File);
        var translations = ShopifyHtmlConverter.MetaFieldsToJson(html, locale.Locale);

        await UpdateIdentifiedContent(translations.ToList());
    }

    [Action("Get metafield",
        Description = "Get metafield details of a specific product")]
    public async Task<MetafieldEntity> GetMetafield(
        [ActionParameter, Display("Metafield"), DataSource(typeof(ProductMetafieldDataHandler))] string metafieldKey,
        [ActionParameter] ProductRequest product)
    {
        var productMetaFields = await GetProductMetafields(product.ProductId);
        return productMetaFields.FirstOrDefault(x => x.Key == metafieldKey) ??
               throw new("No metafield with the provided key found for the project");
    }

    [Action("Update metafield",
        Description = "Update metafield value of a specific product")]
    public async Task UpdateMetafield([ActionParameter] MetafieldRequest metafield,
        [ActionParameter] ProductRequest product, [ActionParameter, Display("New value")] string value)
    {
        var metafieldDefinition = await GetMetafieldDefinitin(metafield.MetafieldDefinitionId);

        if (metafieldDefinition == null)
        {
            throw new PluginApplicationException($"Metafield definition not found for ID:  + {metafield.MetafieldDefinitionId}. Please check the input and try again");
        }

        var request = new ShopifyRestRequest($"/products/{product.ProductId.GetShopifyItemId()}/metafields.json",
                Method.Post, Creds)
            .WithJsonBody(new
            {
                metafield = new
                {
                    value,
                    key = metafieldDefinition.Key,
                    @namespace = metafieldDefinition.Namespace,
                    type = metafieldDefinition.Type.Name,
                }
            });

        await RestClient.ExecuteWithErrorHandling(request);
    }

    private async Task<MetafieldDefinitionEntity> GetMetafieldDefinitin(string metafieldDefinitionId)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.MetafieldDefinition,
            Variables = new
            {
                id = metafieldDefinitionId
            }
        };

        var response = await Client.ExecuteWithErrorHandling<MetafieldDefinitionResponse>(request);
        return response.MetafieldDefinition;
    }

    private async Task<ICollection<MetafieldEntity>> GetProductMetafields(string productId)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceId"] = productId
        };

        var client = new ShopifyClient(Creds, ShopifyClient.GenerateApiUrl(Creds, "unstable"));
        return await client.Paginate<MetafieldEntity, MetafieldPaginationResponse>(GraphQlQueries.ProductMetaFields,
            variables);
    }
}