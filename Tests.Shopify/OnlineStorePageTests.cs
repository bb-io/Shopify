using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Page;

namespace Tests.Shopify;

[TestClass]
public class OnlineStorePageTests : TestBase
{
    [TestMethod]
    public async Task SearchPages_ReturnsPages()
    {
		// Arrange
		var action = new OnlineStorePageActions(InvocationContext, FileManager);
		var input = new SearchPagesRequest
		{
            PublishedAfter = new DateTime(2023, 12, 11, 00, 00, 00, DateTimeKind.Utc),
			PublishedBefore = new DateTime(2023, 12, 13, 00, 00, 00, DateTimeKind.Utc),
        };

		// Act
		var result = await action.SearchPages(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}
}
