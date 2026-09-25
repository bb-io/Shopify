using Apps.Shopify.Constants;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class DeliveryMethodDefinitionService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService
{
    protected override string ContentType => TranslatableResources.DeliveryMethodDefinition;
    
    public Task<FileRecord> Download(DownloadContentRequest input)
    {
        return DownloadTranslatableResource(input);
    }

    public Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        return SearchTranslatableResources(input);
    }
}