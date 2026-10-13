using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.StorePolicy.Api;
using Apps.Shopify.Services.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;

namespace Apps.Shopify.Services.Concrete;

public class StorePolicyService(InvocationContext invocationContext) : BaseTranslatableResourceContentService(invocationContext)
{
    protected override string ContentType => TranslatableResources.StorePolicy;
    
    public override async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        var request = new GraphQLRequest(GraphQlQueries.ShopPolicies);
        var result = await Client.ExecuteWithErrorHandling<StorePolicyOuterResponse>(request);
        var policies = result.Store.StorePolicies;

        if (!string.IsNullOrEmpty(input.NameContains))
            policies = policies.Where(x => x.Title.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();

        var contentItems = policies.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
        return new(contentItems);
    }
}