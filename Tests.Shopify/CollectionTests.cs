using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Collection;

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

	[TestMethod]
	public async Task GetCollectionContent_IsSuccess()
	{
		// Arrange
		var actions = new CollectionActions(InvocationContext, FileManager);
		var collection = new CollectionIdentifier { CollectionId = "gid://shopify/Collection/500406124828" };
		var locale = new LocaleIdentifier { Locale = "en" };
		var outdated = new OutdatedOptionalIdentifier { Outdated = false };

        // Act
		var result = await actions.GetCollectionContent(collection, locale, outdated);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task UpdateCollectionContent_IsSuccess()
	{
        // Arrange
        var actions = new CollectionActions(InvocationContext, FileManager);
		var input = new UploadCollectionRequest
		{
			CollectionId = "gid://shopify/Collection/500406124828",
			File = new FileReference { Name = "test.html" }
		};
		var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

		// Act
		await actions.UpdateCollectionContent(input, locale);
    }
}
