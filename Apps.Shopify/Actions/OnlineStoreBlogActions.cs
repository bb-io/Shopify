using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Blog;
using Apps.Shopify.Models.Identifiers;
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

[ActionList("Blogs")]
public class OnlineStoreBlogActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search blogs", Description = "Search blogs with specific criteria")]
    public async Task<SearchBlogsResponse> SearchBlogs([ActionParameter] SearchBlogsRequest input)
    {
        input.ValidateDates();

        string? query = new QueryBuilder()
            .AddContains("title", input.TitleContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .Build();

        var response = await Client.Paginate<BlogEntity, BlogsPaginationResponse>(
            GraphQlQueries.Blogs,
            QueryHelper.QueryToDictionary(query)
        );

        return new(response);
    }

    [Action("Download blog", Description = "Get content of a specific blog")]
    public async Task<DownloadBlogResponse> GetOnlineStoreBlogTranslationContent(
        [ActionParameter] BlogIdentifier blogId, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] DownloadBlogRequest blogInput,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.BLOG);
        var request = new DownloadContentRequest
        {
            ContentId = blogId.BlogId,
            IncludeBlogPosts = blogInput.IncludeBlogPosts,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload blog", Description = "Upload content of a specific blog")]
    public async Task UpdateOnlineStoreBlogContent(
        [ActionParameter] BlogIdentifier blogId,
        [ActionParameter] UploadBlogRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.BLOG);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = blogId.BlogId,
            Locale = locale.Locale
        };

        await service.Upload(request);
    }
}