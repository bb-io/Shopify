using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Menu;
using ShopifyTests.Base;

namespace Tests.Shopify;

[TestClass]
public class MenuActionTests : TestBase
{
    private MenuActions Actions => new(InvocationContext, FileManager);
    
    [TestMethod]
    public async Task SearchMenus_ReturnsMenus()
    {
        // Arrange
        var input = new SearchMenusRequest
        {
            NameContains = ""
        };

        // Act
        var result = await Actions.SearchMenus(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DownloadMenu_IsSuccess()
    {
        // Arrange
        var menuIdentifier = new MenuIdentifier { MenuId = "gid://shopify/Menu/247055548700" };
        var locale = new LocaleIdentifier { Locale = "fr" };
        var marketIdentifier = new OptionalMarketIdentifier { MarketId = "gid://shopify/Market/94465523996" };
        var outdatedIdentifier = new OutdatedOptionalIdentifier { };

        // Act
        var result = await Actions.DownloadMenu(menuIdentifier, locale, outdatedIdentifier, marketIdentifier);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UploadMenu_IsSuccess()
    {
        // Arrange
        var input = new UploadMenuRequest
        {
            File = new FileReference { Name = "test.html" } 
        };
        var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };
        var marketId = new OptionalMarketIdentifier { MarketId = "gid://shopify/Market/94465523996" };

        // Act
        await Actions.UploadMenu(input, locale, marketId);
    }
}