using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Article;
using Apps.Shopify.Models.Request.OnlineStoreArticle;

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
            UpdatedAfter = DateTime.UtcNow - TimeSpan.FromMinutes(5)
        };

		// Act
		var result = await action.SearchArticles(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public async Task GetOnlineStoreArticleTranslationContent_IsSuccess()
	{
		// Arrange
		var action = new OnlineStoreArticleActions(InvocationContext, FileManager);
		var article = new ArticleIdentifier { ArticleId = "gid://shopify/Article/610979545372" };
		var locale = new LocaleIdentifier { Locale = "en" };
		var outdated = new OutdatedOptionalIdentifier { Outdated = false };

		// Act
		var result = await action.GetOnlineStoreArticleTranslationContent(article, locale, outdated);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task UpdateOnlineStoreArticleContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStoreArticleActions(InvocationContext, FileManager);
		var input = new UploadArticleRequest
		{
			File = new FileReference { Name = "test.html" }
		};
		var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

		// Act
		await action.UpdateOnlineStoreArticleContent(input, locale);
    }
}
