using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Store;
using ShopifyTests.Base;

namespace Tests.Shopify;

[TestClass]
public class StoreTests : TestBase
{
    [TestMethod]
    public async Task GetStoreResourcesContent_IsSuccess()
    {
        // Arrange
        var actions = new StoreActions(InvocationContext, FileManager);
        var resourceType = new ResourceTypeIdentifier { ResourceType = "Metafield" };
        var locale = new LocaleIdentifier { Locale = "en" };
        var outdated = new OutdatedOptionalIdentifier { };

        // Act
        var result = await actions.GetStoreResourcesContent(resourceType, locale, outdated);

        // Assert
        Console.WriteLine(result.Content.Name);
        Assert.IsNotNull(result.Content);
    }

    [TestMethod]
    public async Task DownloadStore_IsSuccess()
    {
        // Arrange
        var actions = new StoreActions(InvocationContext, FileManager);
        var locale = new LocaleIdentifier { Locale = "en" };
        var outdated = new OutdatedOptionalIdentifier { };
        var market = new OptionalMarketIdentifier { };

        // Act
        var result = await actions.DownloadStore(locale, outdated, market);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }

    [TestMethod]
    public Task UploadStore_IsSuccess()
    {
        // Arrange
        var actions = new StoreActions(InvocationContext, FileManager);
        var uploadRequest = new UploadStoreRequest { File = new FileReference { Name = "test.html" } };
        var locale = new NonPrimaryLocaleIdentifier { Locale = "uk" };
        var market = new OptionalMarketIdentifier { };

        // Act
        return actions.UploadStore(uploadRequest, locale, market);
    }
}