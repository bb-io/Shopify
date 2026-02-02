using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Metafield;
using Apps.Shopify.Models.Identifiers;

namespace Tests.Shopify;

[TestClass]
public class MetafieldTests : TestBase
{
    [TestMethod]
    public async Task UpdateMetafiled_IsSuccess()
    {
        var actions = new MetafieldActions(InvocationContext, FileManager);
        var metafiledRequest = new MetafieldDefinitionIdentifier { MetafieldDefinitionId= "gid://shopify/MetafieldDefinition/178835980618" };
        var productRequest = new ProductIdentifier { ProductId = "gid://shopify/Product/15098755907968" };
        string value = "false";

        await actions.UpdateMetafield(metafiledRequest, productRequest, value);
    }

    [TestMethod]
    public async Task SearchMetafields_ReturnsMetafields()
    {
        // Arrange
        var action = new MetafieldActions(InvocationContext, FileManager);
        var input = new SearchMetafieldsRequest
        {
            OwnerType = "PRODUCT",
            NameContains = "mount",
        };

        // Act
        var result = await action.SearchMetafields(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetMetafieldContent_IsSuccess()
    {
        // Arrange
        var action = new MetafieldActions(InvocationContext, FileManager);
        var product = new ProductIdentifier { ProductId = "gid://shopify/Product/10745816351004" };
        var locale = new LocaleIdentifier { Locale = "en" };
        var outdated = new OutdatedOptionalIdentifier { Outdated = false };

        // Act
        var result = await action.GetMetafieldContent(product, locale, outdated);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateMetaFieldContent_IsSuccess()
    {
        // Arrange
        var action = new MetafieldActions(InvocationContext, FileManager);
        var input = new UploadMetafieldRequest
        {
            File = new FileReference { Name = "test.html" },
        };
        var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

        // Act
        await action.UpdateMetaFieldContent(input, locale);
    }
}
