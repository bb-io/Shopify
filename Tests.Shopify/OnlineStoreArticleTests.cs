using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Article;

namespace Tests.Shopify;

[TestClass]
public class OnlineStoreArticleTests : TestBase
{
    [TestMethod]
    public async Task SearchArticles_ReturnsArticles()
    {
		// Arrange
		var action = new OnlineStoreArticleActions(InvocationContext, FileManager);
		var input = new SearchArticlesRequest
		{
            UpdatedAfter = new DateTime(2024, 07, 09, 0, 0, 0, DateTimeKind.Utc),
			UpdatedBefore = new DateTime(2024, 07, 12, 0, 0, 0, DateTimeKind.Utc),
        };

		// Act
		var result = await action.SearchArticles(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}
}
