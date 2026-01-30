using Apps.Shopify.Api;
using Apps.Shopify.Api.Rest;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Metafield;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using RestSharp;

namespace Apps.Shopify.Actions;

[ActionList("Metafields")]
public class MetafieldActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Download metafields", Description = "Get metafield content of a specific product")]
    public async Task<DownloadMetafieldResponse> GetMetafieldContent(
        [ActionParameter] ProductIdentifier resourceRequest,
        [ActionParameter] LocaleIdentifier locale, 
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.METAFIELD);
        var request = new DownloadContentRequest
        {
            ContentId = resourceRequest.ProductId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload metafields", Description = "Upload metafield content of a specific product")]
    public async Task UpdateMetaFieldContent(
        [ActionParameter] UploadMetafieldRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.METAFIELD);
        var request = new UploadContentRequest
        {
            ContentId = input.MetafieldId,
            Content = input.File,
            Locale = locale.Locale
        };

        await service.Upload(request);
    }

    [Action("Search metafield definitions", Description = "Search metafield definitions with specific criteria")]
    public async Task<SearchMetafieldsResponse> SearchMetafields([ActionParameter] SearchMetafieldsRequest input)
    {
        var variables = new Dictionary<string, object> { ["ownerType"] = input.OwnerType };
        string? query = new QueryBuilder()
            .AddEquals("key", input.Key)
            .AddEquals("namespace", input.Namespace)
            .Build();

        var response = await Client.Paginate<MetafieldDefinitionEntity, MetafieldDefinitionPaginationResponse>(
            GraphQlQueries.MetafieldDefinitions,
            QueryHelper.QueryToDictionary(query, variables)
        );

        // We have to filter it by ourselves because API ignores the 'name' filter for some reason
        if (!string.IsNullOrEmpty(input.NameContains))
            response = response.Where(x => x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();

        return new(response);
    }

    [Action("Get metafield", Description = "Get metafield details of a specific product")]
    public async Task<MetafieldEntity> GetMetafield(
        [ActionParameter] MetafieldKeyIdentifier metafieldKey,
        [ActionParameter] ProductIdentifier product)
    {
        var productMetaFields = await GetProductMetafields(product.ProductId);
        return productMetaFields.FirstOrDefault(x => x.Key == metafieldKey.MetafieldKey) ??
               throw new PluginMisconfigurationException("No metafield with the provided key found for the project");
    }

    [Action("Update metafield", Description = "Update metafield value of a specific product")]
    public async Task UpdateMetafield(
        [ActionParameter] MetafieldDefinitionIdentifier metafield,
        [ActionParameter] ProductIdentifier product, 
        [ActionParameter, Display("New value")] string value)
    {
        var metafieldDefinition = await GetMetafieldDefinitin(metafield.MetafieldDefinitionId);

        if (metafieldDefinition == null)
            throw new PluginApplicationException($"Metafield definition not found for ID:  + {metafield.MetafieldDefinitionId}. Please check the input and try again");

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
        var client = new ShopifyClient(Creds, ShopifyClient.GenerateApiUrl(Creds, "unstable"));
        return await client.Paginate<MetafieldEntity, MetafieldPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            new Dictionary<string, object>() { ["resourceId"] = productId }
        );
    }
}