using Apps.Shopify.Api.Rest;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;
using RestSharp;
using System.Net.Mime;

namespace Apps.Shopify.Services.Concrete;

public class BlogService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);

    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId = input.ContentId,
                locale = input.Locale,
                outdated = input.Outdated ?? false
            }
        };
        var blog = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        var blogTranslations = blog.TranslatableResource.GetTranslatableContent();

        var blogPostTranslations = 
            input.IncludeBlogPosts is true
            ? await GetBlogPostTranslations(input.ContentId, input.Locale, input.Outdated ?? default)
            : [];

        var html = ShopifyHtmlConverter.BlogToHtml(blogTranslations.Select(x => new IdentifiedContentEntity(x)
        {
            Id = input.ContentId
        }), blogPostTranslations, HtmlMetadataConstants.OnlineStoreBlogContent);

        return await fileManagementClient.UploadAsync(
            html, 
            MediaTypeNames.Text.Html, 
            $"{input.ContentId.GetShopifyItemId()}.html"
        );
    }

    public async Task Upload(UploadContentRequest input)
    {
        var html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, input.Content);
        var (blogItems, blogPostItems) = ShopifyHtmlConverter.BlogToJson(html, input.Locale);

        if (!string.IsNullOrWhiteSpace(input.ContentId))
        {
            foreach (var item in blogItems)
                item.ResourceId = input.ContentId;
        }

        var allContent = blogItems.Concat(blogPostItems).ToList();
        await _resourceService.UpdateIdentifiedContent(allContent, null);
    }

    private async Task<ICollection<IdentifiedContentEntity>> GetBlogPostTranslations(string blogId, string locale, bool outdated = false)
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
