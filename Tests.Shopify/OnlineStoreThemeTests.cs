using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.OnlineStoreTheme;
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

	[TestMethod]
	public async Task GetOnlineStoreThemeTranslationContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStoreThemeActions(InvocationContext, FileManager);
		var theme = new ThemeIdentifier { ThemeId = "gid://shopify/OnlineStoreTheme/180081623324" };
		var input = new DownloadThemeRequest
		{
			AssetKeys = ["sections.footer"]
		};
		var locale = new LocaleIdentifier { Locale = "en" };
		var outdated = new OutdatedOptionalIdentifier { Outdated = false };

        // Act
		var result = await action.GetOnlineStoreThemeTranslationContent(theme, input, locale, outdated);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }

	[TestMethod]
	public async Task UpdateOnlineStoreThemeContent_IsSuccess()
	{
        // Arrange
        var action = new OnlineStoreThemeActions(InvocationContext, FileManager);
		var input = new UploadThemeRequest
		{
			File = new FileReference { Name = "test.html" }
		};
		var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

        // Act
		await action.UpdateOnlineStoreThemeContent(input, locale);
    }
}
