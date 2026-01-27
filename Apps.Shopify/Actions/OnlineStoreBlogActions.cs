using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Response.Blog;
using Apps.Shopify.Models.Response.TranslatableResource;
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

    [Action("List online store blogs", Description = "List all blogs in the online store")]
    public async Task<ListBlogsResponse> SearchBlogs()
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = TranslatableResource.BLOG
        };
        var response = await Client
            .Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
                GraphQlQueries.TranslatableResources,
                variables);
        return new ListBlogsResponse
        {
            Blogs = response.Select(x => new Blog
            {
                ResourceId = x.ResourceId,
                Title = x.TranslatableContent.First(y => y.Key == "title").Value
            })
        };
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