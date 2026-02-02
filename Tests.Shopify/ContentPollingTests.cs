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
		var date = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromHours(1) };
        var request = new PollingEventRequest<DateMemory> { Memory = date };
        var input = new PollUpdatedContentRequest { };

        // Act
        var result = await polling.OnContentUpdated(request, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
