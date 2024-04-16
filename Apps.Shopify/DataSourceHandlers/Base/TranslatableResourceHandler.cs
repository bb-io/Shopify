using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers.Base;

public abstract class TranslatableResourceHandler : ShopifyInvocable, IAsyncDataSourceHandler
{
    protected abstract TranslatableResource ResourceType { get; }

    public TranslatableResourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = ResourceType
        };
        var response = await Client
            .Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
                GraphQlQueries.TranslatableResources,
                variables, cancellationToken);

        return response
            .Select(x => (x.ResourceId,
                x.TranslatableContent.FirstOrDefault(x => x.Key == "title")?.Value ?? x.ResourceId))
            .Where(x => context.SearchString is null ||
                        x.Item2.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .ToDictionary(x => x.ResourceId, x => x.Item2);
    }
}