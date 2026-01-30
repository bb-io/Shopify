using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Collection;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Collection;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Collection;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Collections")]
public class CollectionActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Download collection", Description = "Get content of a specific collection")]
    public async Task<DownloadCollectionResponse> GetCollectionContent(
        [ActionParameter] CollectionRequest input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.COLLECTION);
        var request = new DownloadContentRequest
        {
            ContentId = input.CollectionId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }
        
    [Action("Upload collection", Description = "Upload content of a specific collection")]
    public async Task UpdateCollectionContent(
        [ActionParameter] UploadCollectionRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.COLLECTION);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = input.CollectionId,
            Locale = locale.Locale
        };

        await service.Upload(request);
    }

    [Action("Search collections", Description = "Search collections with specific criteria")]
    public async Task<SearchCollectionsResponse> SearchCollections([ActionParameter] SearchCollectionsRequest input)
    {
        input.ValidateDates();

        var cleanProductIds = input.ProductIds?
            .Select(id => id.GetShopifyItemId())
            .ToList();

        string? query = new QueryBuilder()
            .AddContains("title", input.TitleContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddOr("product_id", cleanProductIds)
            .Build();

        var response = await Client.Paginate<CollectionEntity, CollectionsPaginationResponse>(
            GraphQlQueries.Collections,
            QueryHelper.QueryToDictionary(query)
        );

        return new(response);
    }
}