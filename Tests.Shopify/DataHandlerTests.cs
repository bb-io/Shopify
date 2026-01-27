using ShopifyTests.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Shopify;

namespace Tests.Shopify;

[TestClass]
public class DataHandlerTests : TestBase
{
    private readonly DataSourceContext _emptyDataSourceContext = new() { SearchString = "Test Product - Blue Shirt" };

    [TestMethod]
    public async Task ContentDataHandler_ReturnsContent()
    {
        // Arrange
        var contentType = new ContentTypeIdentifier { ContentType = TranslatableResource.PRODUCT.ToString() };
        var handler = new ContentDataHandler(InvocationContext, contentType);

        // Act
        var result = await handler.GetDataAsync(_emptyDataSourceContext, default);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
