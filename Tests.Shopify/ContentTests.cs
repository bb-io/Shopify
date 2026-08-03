using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Request.Content;

namespace Tests.Shopify;

[TestClass]
public class ContentTests : TestBase
{
    [TestMethod]
    public async Task SearchContent_ReturnsContent()
    {
		// Arrange
		var action = new ContentActions(InvocationContext, FileManager);
		var input = new SearchContentRequest
		{
			ContentTypes = [TranslatableResources.Metafield]
        };

		// Act
		var result = await action.SearchContent(input);

		// Assert
		PrintJsonResult(result);
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public async Task DownloadContent_IsSuccess()
	{
        // Arrange
        var action = new ContentActions(InvocationContext, FileManager);
		var contentType = new ContentTypeIdentifier { ContentType = "Theme" };
		var input = new DownloadContentRequest
		{
			ContentId = "gid://shopify/OnlineStoreTheme/162863874332",
			Locale = "en",
			MarketId = "gid://shopify/Market/94465523996"
        };

		// Act
		var result = await action.DownloadContent(contentType, input);

        // Assert
        Console.WriteLine(result.Content.Name);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task UploadContent_IsSuccess()
	{
        // Arrange
        var action = new ContentActions(InvocationContext, FileManager);
		var input = new UploadContentRequest
		{
			Content = new FileReference { Name = "test.html" },
			Locale = "fr"
		};

        // Act
		await action.UploadContent(input);
    }
}
