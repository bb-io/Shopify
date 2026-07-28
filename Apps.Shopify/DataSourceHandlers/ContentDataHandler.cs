using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ContentDataHandler : ShopifyInvocable, IAsyncDataSourceItemHandler
{
    private readonly ContentTypeIdentifier _contentType;

    public ContentDataHandler(InvocationContext context, [ActionParameter] ContentTypeIdentifier contentType) : base(context)
    {
        if (string.IsNullOrEmpty(contentType.ContentType))
            throw new PluginMisconfigurationException("Please specify content type first.");
        
        _contentType = contentType;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var variables = new Dictionary<string, object>
        {
            ["resourceType"] = TranslatableResources.GetApiType(_contentType.ContentType),
        };

        var response = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            variables, 
            ct
        );

        return response
            .Select(x => new DataSourceItem(
                x.ResourceId,
                x.TranslatableContent.FirstOrDefault(t => t.Key == "title")?.Value ?? x.ResourceId)
            )
            .Where(x =>
                context.SearchString is null ||
                x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();
    }
}
