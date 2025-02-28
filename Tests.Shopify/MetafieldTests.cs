using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Metafield;
using Apps.Shopify.Models.Request.Product;
using ShopifyTests.Base;

namespace Tests.Shopify
{
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

            Assert.IsTrue(true);
        }
    }
}
