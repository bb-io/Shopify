using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Api.Rest;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.Blog;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using RestSharp;

namespace Apps.Shopify.Actions;

[ActionList]
public class OnlineStoreBlogActions : TranslatableResourceActions
{
    public OnlineStoreBlogActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }

    [Action("List online store blogs", Description = "List all blogs in the online store")]
    public async Task<ListBlogsResponse> SearchBlogs()
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = TranslatableResource.ONLINE_STORE_BLOG
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

    [Action("Get online store blog content as HTML",
        Description = "Get content of a specific online store blog in HTML format")]
    public async Task<FileResponse> GetOnlineStoreBlogTranslationContent(
        [ActionParameter] OnlineStoreBlogRequest input, [ActionParameter] LocaleRequest locale,
        [ActionParameter, Display("Include articles")]
        bool? includeBlogsPosts,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = input.OnlineStoreBlogId,
                locale = locale.Locale,
                outdated = getContentRequest.Outdated ?? false
            }
        };
        var blog = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        var blogTranslations = blog.TranslatableResource.GetTranslatableContent();

        var blogPostTranslations = includeBlogsPosts is true
            ? await GetBlogPostTranslations(input.OnlineStoreBlogId, locale.Locale,
                getContentRequest.Outdated ?? default)
            : [];

        var html = ShopifyHtmlConverter.BlogToHtml(blogTranslations.Select(x => new IdentifiedContentEntity(x)
        {
            Id = input.OnlineStoreBlogId
        }), blogPostTranslations);

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"{input.OnlineStoreBlogId.GetShopifyItemId()}.html")
        };
    }

    [Action("Update online store blog content from HTML",
        Description = "Update content of a specific online store blog from HTML file")]
    public async Task UpdateOnlineStoreBlogContent([ActionParameter] OnlineStoreBlogRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
    {
        var fileStream = await FileManagementClient.DownloadAsync(file.File);
        var (blog, blogPosts) = ShopifyHtmlConverter.BlogToJson(fileStream, locale.Locale);

        await UpdateBlogContent(resourceRequest.OnlineStoreBlogId, blog.ToList());

        var blogPostsContent = blogPosts.ToList();
        if (blogPostsContent.Any())
            await UpdateIdentifiedContent(blogPostsContent);
    }

    private async Task UpdateBlogContent(string blogId, ICollection<IdentifiedContentRequest> blogContents)
    {
        if (blogContents.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
        {
            var sourceContent = await GetResourceSourceContent(blogId);
            blogContents.ToList().ForEach(x =>
                x.TranslatableContentDigest = sourceContent.TranslatableResource.TranslatableContent
                    .First(y => y.Key == x.Key).Digest);
        }

        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.TranslationsRegister,
            Variables = new
            {
                resourceId = blogId,
                translations = blogContents.Select(x => new TranslatableResourceContentRequest(x))
            }
        };

        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task<ICollection<IdentifiedContentEntity>> GetBlogPostTranslations(string blogId, string locale,
        bool outdated = false)
    {
        var request = new ShopifyRestRequest($"blogs/{blogId.GetShopifyItemId()}/articles.json", Method.Get, Creds);
        var response = await new ShopifyRestClient(Creds)
            .Paginate<OnlineStoreArticleEntity, ArticlesPaginationResponse>(request);

        var ids = response.Select(x => x.Id).ToArray();

        var variables = new Dictionary<string, object>()
        {
            ["resourceIds"] = ids,
            ["locale"] = locale,
            ["outdated"] = outdated
        };

        var content = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcesByIdsPaginationResponse>(
            GraphQlQueries.TranslatableResourcesByIds,
            variables);

        return content
            .Select(x => (x.ResourceId, x.GetTranslatableContent()))
            .SelectMany(x => x.Item2.Select(y => new IdentifiedContentEntity(y)
            {
                Id = x.ResourceId
            }))
            .ToList();
    }
}