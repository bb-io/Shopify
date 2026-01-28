using ShopifyTests.Base;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Request.Metafield;

namespace Tests.Shopify;

[TestClass]
public class MetafieldTests : TestBase
{
    [TestMethod]
    public async Task UpdateMetafiled_IsSuccess()
    {
        var actions = new MetafieldActions(InvocationContext, FileManager);
        var metafiledRequest = new MetafieldRequest { MetafieldDefinitionId= "gid://shopify/MetafieldDefinition/178835980618" };
        var productRequest = new ProductRequest { ProductId = "gid://shopify/Product/15098755907968" };
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
}
