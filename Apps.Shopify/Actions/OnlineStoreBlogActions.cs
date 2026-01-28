using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Blog;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Blog;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Response.Blog;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Online store blogs")]
public class OnlineStoreBlogActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search online store blogs", Description = "Search online store blogs with specific criteria")]
    public async Task<SearchBlogsResponse> SearchBlogs([ActionParameter] SearchBlogsRequest input)
    {
        var queryParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(input.TitleContains))
            queryParts.Add($"title:*{input.TitleContains}*");

        if (input.UpdatedAfter.HasValue)
            queryParts.Add($"updated_at:>{input.UpdatedAfter.Value:O}");

        if (input.UpdatedBefore.HasValue)
            queryParts.Add($"updated_at:<{input.UpdatedBefore.Value:O}");

        if (input.CreatedAfter.HasValue)
            queryParts.Add($"created_at:>{input.CreatedAfter.Value:O}");

        if (input.CreatedBefore.HasValue)
            queryParts.Add($"created_at:<{input.CreatedBefore.Value:O}");

        var variables = new Dictionary<string, object>();
        if (queryParts.Count != 0)
            variables["query"] = string.Join(" AND ", queryParts);

        var response = await Client.Paginate<BlogEntity, BlogsPaginationResponse>(
            GraphQlQueries.Blogs,
            variables
        );

        return new(response);
    }

    [Action("Download online store blog", Description = "Get content of a specific online store blog")]
    public async Task<DownloadBlogResponse> GetOnlineStoreBlogTranslationContent(
        [ActionParameter] OnlineStoreBlogRequest input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter, Display("Include articles")] bool? includeBlogsPosts,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.BLOG);
        var request = new DownloadContentRequest
        {
            ContentId = input.OnlineStoreBlogId,
            IncludeBlogPosts = includeBlogsPosts,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload online store blog", Description = "Upload content of a specific online store blog")]
    public async Task UpdateOnlineStoreBlogContent(
        [ActionParameter] UploadBlogRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.BLOG);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = input.BlogId,
            Locale = locale.Locale
        };

        await service.Upload(request);
    }
}