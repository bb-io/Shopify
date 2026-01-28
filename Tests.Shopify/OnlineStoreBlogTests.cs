using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Blog;
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
}
