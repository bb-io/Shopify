using ShopifyTests.Base;
using Apps.Shopify.Polling;
using Apps.Shopify.Polling.Models.Memory;
using Apps.Shopify.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Tests.Shopify;

[TestClass]
public class ContentPollingTests : TestBase
{
    [TestMethod]
    public async Task OnContentUpdated_ReturnsUpdatedContent()
    {
		// Arrange
		var polling = new ContentPollingList(InvocationContext);
		var digests = new Dictionary<string, string>
		{
			{ "gid://shopify/Menu/247055548700", "382569A684C6656594C038EC118EBB01D4356E3AD3E2D7CBA7117810F3BA58D3" },
			{ "gid://shopify/Menu/247055581468", "0C51C2FEC556EA53B8BBE6C00058F0DD1CC0B3F96A6BEF0BECBC14F68F1D5391" },
			{ "gid://shopify/Menu/255319539996", "7F8218301B671A04DBF2BA14BE7EDE204622141A0A7019E5327670061B28DBA5" },
			{ "gid://shopify/Menu/275676037404", "2AFFF611EDA0DB3C79EE078916A118FBCFF6095311F30BE13BD603DA0F00BE89" }
		};
		var memory = new DigestDateMemory
		{
			LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5),
			Digests = digests
		};
        var request = new PollingEventRequest<DigestDateMemory> { Memory = memory };
        var input = new PollUpdatedContentRequest { ContentTypes = ["Menu"] };

        // Act
        var result = await polling.OnContentUpdated(request, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
