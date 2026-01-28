using ShopifyTests.Base;
using Apps.Shopify.Actions;
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
            //MetafieldKey = "test_data.snowboard_length", 
            //MetafieldValueContains = "100",
            CreatedAfter = new DateTime(2026, 01, 27, 10, 0, 0, DateTimeKind.Utc),
            CreatedBefore = new DateTime(2026, 01, 29, 10, 0, 0, DateTimeKind.Utc),
        };

        // Act
        var response = await action.SearchProducts(searchProductsRequest);

        // Assert
        PrintJsonResult(response);
        Assert.IsNotNull(response);
    }
}
