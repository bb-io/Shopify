using Apps.Shopify.Constants;
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
			{
				"gid://shopify/PaymentGateway/171033624903", "before_payment_instructions:b0b10a317235fae75f779596ea76a42ab50e51f16b6c80be495e1ee030d4deef|message:2b3b7c4f195b0fce775ee72605c0964fa9006f31c4c826772bfc5e26377438c3|name:e04c8ae34c303c8b36be8dd65a2f45beb7376c97baa7a7211563dad339cc2dc5"
			},
		};
		var memory = new DigestDateMemory
		{
			LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5),
			Digests = digests
		};
        var request = new PollingEventRequest<DigestDateMemory> { Memory = memory };
        var input = new PollUpdatedContentRequest { ContentTypes = [TranslatableResources.PaymentGateway] };

        // Act
        var result = await polling.OnContentUpdated(request, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
