using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request.Product;
using ShopifyTests.Base;

namespace Tests.Shopify
{
    [TestClass]
    public class ProductTests : TestBase
    {
        [TestMethod]
        public async Task SearchProduct_IsSuccess()
        {
            var actions = new ProductActions(InvocationContext, FileManager);

            var searchProductsRequest = new SearchProductsRequest { MetafieldKey = "translate", MetafieldValue="true" };

            var response = await actions.SearchProducts(searchProductsRequest);

            Assert.IsNotNull(response);
        }
    }
}
