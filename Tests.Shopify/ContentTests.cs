using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Identifiers;

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
			ContentTypes = ["PRODUCT"]
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
		var contentType = new ContentTypeIdentifier { ContentType = "PRODUCT" };
		var input = new DownloadContentRequest
		{
			ContentId = "gid://shopify/Product/10745816351004",
			Locale = "en"
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
