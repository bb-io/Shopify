using Apps.Shopify.Constants;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class DeliveryMethodDefinitionService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService, IDigestPollingContentService
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

    public Task Upload(UploadContentServiceRequest input)
    {
        return UploadTranslatableResource(input);
    }

    public Task<DigestPollResult> PollUpdated(IReadOnlyDictionary<string, string> knownDigests, PollUpdatedContentRequest input)
    {
        return PollTranslatableResourceDigests(knownDigests, input);
    }
}