using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.OnlineStore;
using ShopifyTests.Base;

namespace Tests.Shopify
{
    [TestClass]
    public class StoreTests : TestBase
    {
        [TestMethod]
        public async Task GetStoreContent_ReturnsValues()
        {
            var action = new StoreActions(InvocationContext,FileManager);
            var input1 = new StoreContentRequest { IncludeShop =true};
            var input2= new LocaleRequest { Locale = "fr" };
            var input3 = new GetContentRequest { };
            var result = await action.GetStoreContent(input1,input2,input3);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ListPages_ReturnsValues()
        {
            var action = new OnlineStorePageActions(InvocationContext, FileManager);
            var input1 = new StoreContentRequest { IncludeShop = true };
            var input2 = new LocaleRequest { Locale = "fr" };
            var input3 = new GetContentRequest { };
            var result = await action.ListPages();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }
    }
}
