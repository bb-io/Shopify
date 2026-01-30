using ShopifyTests.Base;
using Apps.Shopify;
using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Tests.Shopify;

[TestClass]
public class DataHandlerTests : TestBase
{
    private readonly DataSourceContext _emptyDataSourceContext = new() { SearchString = "" };

    private async Task TestHandler<T>(params object[] additionalArgs) where T : class
    {
        // Arrange
        var constructorArgs = new object[] { InvocationContext }.Concat(additionalArgs).ToArray();
        var handler = (dynamic)Activator.CreateInstance(typeof(T), constructorArgs)!;

        // Act
        var result = await handler.GetDataAsync(_emptyDataSourceContext, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ContentDataHandler_ReturnsContent()
    {
        var contentType = new ContentTypeIdentifier
        {
            ContentType = TranslatableResource.PRODUCT.ToString()
        };

        await TestHandler<ContentDataHandler>(contentType);
    }

    [TestMethod]
    public async Task ProductDataHandler_ReturnsProducts() => await TestHandler<ProductDataHandler>();

    [TestMethod]
    public async Task ProductMetafieldKeyDataHandler_ReturnsMetafieldKeys() 
        => await TestHandler<ProductMetafieldKeyDataHandler>();

    [TestMethod]
    public async Task ArticleDataHandler_ReturnsArticles() => await TestHandler<ArticleDataHandler>();

    [TestMethod]
    public async Task ThemeDataHandler_ReturnsThemes() => await TestHandler<ThemeDataHandler>();

    [TestMethod]
    public async Task AssetThemeDataHandler_ReturnsAssetThemes()
    {
        // Arrange
        var theme = new ThemeIdentifier { /*ThemeId = "gid://shopify/OnlineStoreTheme/162863874332"*/ };
        var contentRequest = new DownloadContentRequest { ContentId = "gid://shopify/OnlineStoreTheme/162863874332" };

        // Act
        await TestHandler<AssetThemeDataHandler>(theme, contentRequest);
    }

    [TestMethod]
    public async Task BlogDataHandler_ReturnsBlogs() => await TestHandler<BlogDataHandler>();

    [TestMethod]
    public async Task CollectionDataHandler_ReturnsCollections() => await TestHandler<CollectionDataHandler>();

    [TestMethod]
    public async Task LanguageDataHandler_ReturnsLanguages() => await TestHandler<LanguageDataHandler>();

    [TestMethod]
    public async Task MetafieldDataHandler_ReturnsMetafields() => await TestHandler<MetafieldDataHandler>();

    [TestMethod]
    public async Task NonPrimaryLanguageDataHandler_ReturnsNonPrimaryLanguages() 
        => await TestHandler<NonPrimaryLanguageDataHandler>();

    [TestMethod]
    public async Task PageDataHandler_ReturnsPages() => await TestHandler<PageDataHandler>();

    [TestMethod]
    public async Task ProductMetafieldDefinitionDataHandler_ReturnsProductMetafieldDefinitions() 
        => await TestHandler<ProductMetafieldDefinitionDataHandler>();
}
