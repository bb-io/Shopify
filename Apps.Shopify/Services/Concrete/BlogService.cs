using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Entities.Blog;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.Blog;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using System.Net.Mime;
using Apps.Shopify.HtmlConversion.Models;

namespace Apps.Shopify.Services.Concrete;

public class BlogService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService, IPollingContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);
    private readonly string _contentType = TranslatableResources.Blog;

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var request = new GraphQLRequest
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = input.ContentId,
                locale = input.Locale,
                outdated = input.Outdated ?? false,
                marketId = input.MarketId
            }
        };
        var blog = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        var blogTranslations = blog.TranslatableResource.GetTranslatableContent();
        
        var metadata = new ShopifyMetadata
        {
            ContentType = _contentType.ToLower(),
            MarketId = input.MarketId
        };
        
        var blogPostTranslations = input.IncludeBlogPosts is true
            ? await GetBlogPostTranslations(input.ContentId, input.Locale, metadata, input.Outdated ?? false)
            : [];
        
        var html = ShopifyHtmlConverter.BlogToHtml(
            blogTranslations.Select(x => new IdentifiedContentEntity(x) { Id = input.ContentId }), 
            blogPostTranslations,
            metadata);

        return await fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, input.ContentId.GetFileName(metadata.MarketId));
    }

    public async Task<ContentUpdatedResponse> PollUpdated(DateTime after, DateTime before, PollUpdatedContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", after, before)
            .Build();

        var response = await Client.Paginate<BlogEntity, BlogsPaginationResponse>(
            GraphQlQueries.Blogs,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => 
            new PollingContentItemEntity(x.Id, _contentType, x.Title, x.UpdatedAt ?? x.CreatedAt)
        ).ToList();
        return new(items);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .AddDateRange("updated_at", input.UpdatedAfter, input.UpdatedBefore)
            .AddDateRange("created_at", input.CreatedAfter, input.CreatedBefore)
            .Build();

        var response = await Client.Paginate<BlogEntity, BlogsPaginationResponse>(
            GraphQlQueries.Blogs,
            QueryHelper.QueryToDictionary(query)
        );

        var items = response.Select(x => new ContentItemEntity(x.Id, _contentType, x.Title)).ToList();
        return new(items);
    }

    public async Task Upload(UploadContentRequest input)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, input.Content);
        var metadata = new ShopifyMetadata
        {
            ContentType = _contentType,
            MarketId = input.MarketId
        };
        
        var (blogItems, blogPostItems) = ShopifyHtmlConverter.BlogToJson(html, input.Locale, metadata);

        if (!string.IsNullOrWhiteSpace(input.ContentId))
        {
            foreach (var item in blogItems)
                item.ResourceId = input.ContentId;
        }

        var allContent = blogItems.Concat(blogPostItems).ToList();
        await _resourceService.UpdateIdentifiedContent(allContent, null, metadata.MarketId);
    }

    private async Task<ICollection<IdentifiedContentEntity>> GetBlogPostTranslations(
        string blogId, 
        string locale, 
        ShopifyMetadata metadata,
        bool outdated = false)
    {
        var variables = new Dictionary<string, object>
        {
            ["query"] = $"blog_id:{blogId.GetShopifyItemId()}"
        };

        var articlesResponse = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            variables
        );

        if (articlesResponse.Count == 0)
            return [];

        var articleIds = articlesResponse.Select(x => x.Id).ToArray();

        var translationVariables = new Dictionary<string, object>
        {
            ["resourceIds"] = articleIds,
            ["locale"] = locale,
            ["outdated"] = outdated
        };

        if (!string.IsNullOrEmpty(metadata.MarketId))
            translationVariables["marketId"] = metadata.MarketId;

        var content = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcesByIdsPaginationResponse>(
            GraphQlQueries.TranslatableResourcesByIds,
            translationVariables
        );

        return content
            .Select(x => (x.ResourceId, x.GetTranslatableContent()))
            .SelectMany(x => x.Item2.Select(y => new IdentifiedContentEntity(y)
            {
                Id = x.ResourceId
            }))
            .ToList();
    }
}
