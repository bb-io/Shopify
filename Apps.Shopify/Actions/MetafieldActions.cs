using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Metafield;
using Apps.Shopify.Models.Response.Metafield;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions;

[ActionList("Metafields")]
public class MetafieldActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext);
    protected override string ContentType => TranslatableResources.Metafield;

    [Action("Download metafields", Description = "Download metafield content of a specific product")]
    public async Task<DownloadMetafieldResponse> GetMetafieldContent(
        [ActionParameter] ProductIdentifier resourceRequest,
        [ActionParameter] LocaleIdentifier locale, 
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier,
        [ActionParameter, Display("Name contains")] string? nameContains = null)
    {
        var metafields = await _resourceService.GetProductMetafields(
            resourceRequest.ProductId,
            locale.Locale,
            getContentRequest.Outdated ?? false,
            marketIdentifier.MarketId,
            nameContains: nameContains);

        var metadata = new ShopifyMetadata
        {
            ContentType = ContentType.ToLower(),
            MarketId = marketIdentifier.MarketId
        };

        var html = ShopifyHtmlConverter.ToHtml(metafields, metadata);
        var fileRecord = new FileRecord(html, MediaTypeNames.Text.Html, resourceRequest.ProductId.GetFileName(marketIdentifier.MarketId));
        
        var file = await fileManagementClient.UploadFileRecord(fileRecord);
        return new(file);
    }

    [Action("Upload metafields", Description = "Upload metafield content of a specific product")]
    public Task UpdateMetaFieldContent(
        [ActionParameter] UploadMetafieldRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.MetafieldId, locale.Locale, marketIdentifier.MarketId);
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
        var productMetaFields = await Client.Paginate<MetafieldEntity, ProductMetafieldsPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            new Dictionary<string, object> { ["resourceId"] = product.ProductId }
        );
        
        return productMetaFields.FirstOrDefault(x => x.ToString() == metafieldKey.MetafieldKey) ??
               throw new PluginMisconfigurationException("No metafield with the provided key found for the product");
    }

    [Action("Update metafield", Description = "Update metafield value of a specific product")]
    public async Task UpdateMetafield(
        [ActionParameter] MetafieldDefinitionIdentifier metafield,
        [ActionParameter] ProductIdentifier product, 
        [ActionParameter, Display("New value")] string value)
    {
        var metafieldDefinition = await GetMetafieldDefinition(metafield.MetafieldDefinitionId);

        if (metafieldDefinition == null)
        {
            throw new PluginApplicationException(
                $"Metafield definition not found for ID:  + {metafield.MetafieldDefinitionId}. " +
                $"Please check the input and try again"
            );
        }

        var variables = new
        {
            metafields = new[]
            {
                new
                {
                    ownerId = product.ProductId,
                    @namespace = metafieldDefinition.Namespace,
                    key = metafieldDefinition.Key,
                    type = metafieldDefinition.Type.Name,
                    value
                }
            }
        };

        var request = new GraphQLRequest
        {
            Query = GraphQlMutations.MetafieldsSet,
            Variables = variables
        };

        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task<MetafieldDefinitionEntity> GetMetafieldDefinition(string metafieldDefinitionId)
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
}
