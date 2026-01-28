using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Collection;
using ShopifyTests.Base;

namespace Tests.Shopify;

[TestClass]
public class CollectionTests : TestBase
{
    [TestMethod]
    public async Task SearchCollections_ReturnsCollections()
    {
		// Arrange
		var actions = new CollectionActions(InvocationContext, FileManager);
		var input = new SearchCollectionsRequest
		{
			TitleContains = "Collection"
		};

		// Act
		var result = await actions.SearchCollections(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}
}
