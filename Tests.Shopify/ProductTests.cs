using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Shopify.Actions;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Product;
using Blackbird.Applications.Sdk.Common.Files;
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

        [TestMethod]
        public async Task Update_product()
        {
            var actions = new ProductActions(InvocationContext, FileManager);
            var file = new FileReference() { Name = "10510521631004.html.xlf" };
            await actions.UpdateProductContent(new NonPrimaryLocaleRequest { Locale = "nl" }, new FileRequest { File = file });
        }
    }
}
