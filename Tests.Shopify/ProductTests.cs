using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Product;

namespace Tests.Shopify;

[TestClass]
public class ProductTests : TestBase
{
    [TestMethod]
    public async Task SearchProducts_ReturnsProducts()
    {
        // Arrange
        var action = new ProductActions(InvocationContext, FileManager);
        var searchProductsRequest = new SearchProductsRequest
        {
            UpdatedAfter = DateTime.UtcNow - TimeSpan.FromHours(1),
            UpdatedBefore = DateTime.UtcNow
        };

        // Act
        var response = await action.SearchProducts(searchProductsRequest);

        // Assert
        PrintJsonResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetProductTranslationContent_IsSuccess()
    {
        // Arrange
        var action = new ProductActions(InvocationContext, FileManager);
        var product = new ProductIdentifier { ProductId = "gid://shopify/Product/10745816351004" };
        var locale = new LocaleIdentifier { Locale = "en" };
        var input = new DownloadProductRequest { };
        var outdated = new OutdatedOptionalIdentifier { Outdated = false };

        // Act
        var response = await action.GetProductTranslationContent(product, locale, input, outdated);

        // Assert
        Console.WriteLine(response.File.Name);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task UpdateProductContent_IsSuccess()
    {
        // Arrange
        var action = new ProductActions(InvocationContext, FileManager);
        var input = new UploadProductRequest
        { 
            File = new FileReference { Name = "test.html" } 
        };
        var locale = new NonPrimaryLocaleIdentifier { Locale = "nl" };

        // Act
        await action.UpdateProductContent(input, locale);
    }
}
