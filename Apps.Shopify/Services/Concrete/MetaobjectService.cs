using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Metaobject;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Metaobject;
using Apps.Shopify.Services.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class MetaobjectService(InvocationContext invocationContext) : BaseTranslatableResourceContentService(invocationContext)
{
    protected override string ContentType => TranslatableResources.Metaobject;
    
    public override async Task<SearchContentResponse> Search(
        SearchContentRequest input,
        bool firstPageOnly = false,
        CancellationToken token = default)
    {
        var byEntryName = await base.Search(input, firstPageOnly, token);
        if (string.IsNullOrWhiteSpace(input.NameContains))
            return byEntryName;
        
        var definitions = await Client.Paginate<MetaobjectDefinitionEntity, MetaobjectDefinitionsPaginationResponse>(
            GraphQlQueries.MetaobjectDefinitions,
            new Dictionary<string, object>(),
            firstPageOnly,
            token);
        
        var matchingDefinitions = definitions.Where(x => x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));
        var contentItems = new List<ContentItemEntity>();
        foreach (var definition in matchingDefinitions)
        {
            var metaobjects = await Client.Paginate<MetaobjectEntity, MetaobjectsPaginationResponse>(
                GraphQlQueries.MetaobjectsByType,
                new Dictionary<string, object> { ["type"] = definition.Type },
                firstPageOnly,
                token);

            contentItems.AddRange(metaobjects.Select(x => new ContentItemEntity(x.Id, ContentType, $"{x.DisplayName} ({definition.Name})")));
        }
        
        var items = byEntryName.Items
            .Concat(contentItems)
            .DistinctBy(x => x.ContentId)
            .ToList();

        return new(items);
    }
}