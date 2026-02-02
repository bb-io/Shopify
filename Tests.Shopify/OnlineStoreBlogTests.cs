using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Blog;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using ShopifyTests.Base;

namespace Tests.Shopify;

[TestClass]
public class OnlineStoreBlogTests : TestBase
{
    [TestMethod]
    public async Task SearchBlogs_ReturnsBlogs()
    {
		// Arrange
		var action = new OnlineStoreBlogActions(InvocationContext, FileManager);
		var input = new SearchBlogsRequest
		{
			UpdatedAfter = DateTime.UtcNow - TimeSpan.FromHours(1)
		};

		// Act
		var result = await action.SearchBlogs(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public async Task GetOnlineStoreBlogTranslationContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStoreBlogActions(InvocationContext, FileManager);
		var blog = new BlogIdentifier { BlogId = "gid://shopify/Blog/117694071068" };
		var locale = new LocaleIdentifier { Locale = "en" };
		var input = new DownloadBlogRequest
		{
			IncludeBlogPosts = true
		};
		var outdated = new OutdatedOptionalIdentifier { Outdated = false };

        // Act
		var result = await action.GetOnlineStoreBlogTranslationContent(blog, locale, input, outdated);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task UpdateOnlineStoreBlogContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStoreBlogActions(InvocationContext, FileManager);
		var input = new UploadBlogRequest
		{
			File = new FileReference { Name = "test.html" }
		};
		var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

        // Act
		await action.UpdateOnlineStoreBlogContent(input, locale);
    }
}
