using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Entities.Blog;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Blog;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Response.Blog;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Blogs")]
public class OnlineStoreBlogActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.Blog;

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

    [Action("Download blog", Description = "Download content of a specific blog")]
    public async Task<DownloadBlogResponse> GetOnlineStoreBlogTranslationContent(
        [ActionParameter] BlogIdentifier blogId, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] DownloadBlogRequest blogInput,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = blogId.BlogId,
            IncludeBlogPosts = blogInput.IncludeBlogPosts,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }

    [Action("Upload blog", Description = "Upload content of a specific blog")]
    public Task UpdateOnlineStoreBlogContent(
        [ActionParameter] UploadBlogRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.BlogId, locale.Locale, marketIdentifier.MarketId);
    }
}