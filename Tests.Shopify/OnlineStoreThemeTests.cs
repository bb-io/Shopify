using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Theme;
using ShopifyTests.Base;

namespace Tests.Shopify;

[TestClass]
public class OnlineStoreThemeTests : TestBase
{
    [TestMethod]
    public async Task SearchThemes_ReturnsThemes()
    {
		// Arrange
		var action = new OnlineStoreThemeActions(InvocationContext, FileManager);
		var input = new SearchThemesRequest
		{
			Role = "UNPUBLISHED"
        };

		// Act
		var result = await action.SearchThemes(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}
}
