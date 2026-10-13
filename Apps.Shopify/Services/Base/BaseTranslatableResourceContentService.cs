using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Base;

public abstract class BaseTranslatableResourceContentService(InvocationContext invocationContext) 
    : BaseContentService(invocationContext), IContentService, IDigestPollingContentService
{
    public virtual Task<FileRecord> Download(DownloadContentRequest input)
    {
        return DownloadTranslatableResource(input);
    }

    public virtual Task<SearchContentResponse> Search(SearchContentRequest input, bool firstPageOnly = false, CancellationToken token = default)
    {
        return SearchTranslatableResources(input, firstPageOnly, token);
    }

    public virtual Task Upload(UploadContentServiceRequest input)
    {
        return UploadTranslatableResource(input);
    }

    public Task<DigestPollResult> PollUpdated(IReadOnlyDictionary<string, string> knownDigests, PollUpdatedContentRequest input)
    {
        return PollTranslatableResourceDigests(knownDigests, input);
    }
}