using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
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
}