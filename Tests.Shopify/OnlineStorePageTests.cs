using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Page;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.OnlineStorePage;

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
            //PublishedAfter = new DateTime(2023, 12, 11, 00, 00, 00, DateTimeKind.Utc),
			//PublishedBefore = new DateTime(2023, 12, 13, 00, 00, 00, DateTimeKind.Utc),
        };

		// Act
		var result = await action.SearchPages(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public async Task GetOnlineStorePageTranslationContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStorePageActions(InvocationContext, FileManager);
		var page = new PageIdentifier { PageId = "gid://shopify/Page/151751328028" };
		var locale = new LocaleIdentifier { Locale = "en" };
		var outdated = new OutdatedOptionalIdentifier { Outdated = false };

        // Act
		var result = await action.GetOnlineStorePageTranslationContent(page, locale, outdated);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task UpdateOnlineStorePageContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStorePageActions(InvocationContext, FileManager);
		var input = new UploadPageRequest 
		{ 
			File = new FileReference { Name = "test.html" } 
		};
		var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

		// Act
		await action.UpdateOnlineStorePageContent(input, locale);
    }
}
