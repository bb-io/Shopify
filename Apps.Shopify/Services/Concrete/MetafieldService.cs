using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Response.TranslatableResource;

namespace Apps.Shopify.Services.Concrete;

public class MetafieldService(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);
    private readonly string _contentType = TranslatableResources.Metafield;

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        return await _resourceService.GetResourceContent(
            input.ContentId,
            input.Locale,
            input.Outdated ?? false,
            _contentType.ToLower()
        );
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        var variables = new Dictionary<string, object>
        {
            ["resourceType"] = TranslatableResources.GetApiType(_contentType)
        };

        var response = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            variables
        );

        var items = response
            .Select(x => new ContentItemEntity(x.ResourceId, _contentType, x.ToString()))
            .Where(x => string.IsNullOrEmpty(input.NameContains) ||
                        x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase) ||
                        x.ContentId.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        return new(items);
    }

    public async Task Upload(UploadContentRequest input)
    {
        await _resourceService.UpdateResourceContent(input.ContentId, input.Locale, input.Content);
    }
}
