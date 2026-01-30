using Apps.Shopify.Api;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Metafield;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Metafield;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.Net.Mime;

namespace Apps.Shopify.Services.Concrete;

public class MetafieldService(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var productMetaFields = await GetProductMetafields(input.ContentId);
        var metaFields = await _resourceService.ListTranslatableResources(
            TranslatableResource.METAFIELD, 
            input.Locale, 
            input.Outdated ?? default
        );

        var resources = metaFields
            .Where(x => productMetaFields.Any(y => x.ResourceId == y.Id))
            .ToArray();

        var contents = resources.All(x => !x.Translations.Any())
            ? resources.Select(x => (x.ResourceId, x.TranslatableContent.FirstOrDefault())).ToArray()
            : resources.Select(x => (x.ResourceId, x.Translations.FirstOrDefault())).ToArray();

        var html = ShopifyHtmlConverter.MetaFieldsToHtml(
            contents.Where(x => x.Item2 is not null), 
            HtmlMetadataConstants.MetafieldContent
        );
        return await fileManagementClient.UploadAsync(
            html, 
            MediaTypeNames.Text.Html,
            $"{input.ContentId.GetShopifyItemId()}-metafields.html"
        );
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        var variables = new Dictionary<string, object> { ["ownerType"] = input.MetafieldOwnerType! };

        var response = await Client.Paginate<MetafieldDefinitionEntity, MetafieldDefinitionPaginationResponse>(
            GraphQlQueries.MetafieldDefinitions,
            variables
        );

        if (!string.IsNullOrEmpty(input.NameContains))
            response = response.Where(x => x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();

        var items = response.Select(x => new ContentItemEntity(x.Id, "Metafield", x.Name)).ToList();
        return new(items);
    }

    public async Task Upload(UploadContentRequest input)
    {
        await _resourceService.UpdateResourceContent(input.ContentId, input.Locale, input.Content);
    }

    private async Task<ICollection<MetafieldEntity>> GetProductMetafields(string productId)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceId"] = productId
        };

        var client = new ShopifyClient(Creds, ShopifyClient.GenerateApiUrl(Creds, "unstable"));
        return await client.Paginate<MetafieldEntity, MetafieldPaginationResponse>(
            GraphQlQueries.ProductMetaFields,
            variables
        );
    }
}
