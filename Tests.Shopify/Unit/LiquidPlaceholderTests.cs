using Apps.Shopify.HtmlConversion.Models;

namespace Tests.Shopify.Unit;

[TestClass]
public class LiquidPlaceholderTests
{
    [TestMethod]
    [DataRow("<a href=\"{{ routes.root_url }}\">Shop {{ shop.name }}</a>")]
    [DataRow("<td{% if x %} class=\"a\"{% endif %}>Hi</td>")]
    [DataRow("Gift card code {{code}} is invalid.")]
    public void LockHtml_RoundTrips(string html)
    {
        // Act
        string locked = LiquidPlaceholder.LockHtml(html);

        // Assert
        Assert.DoesNotContain("href=\"<span", locked);
        Assert.AreEqual(html, LiquidPlaceholder.Unlock(locked));
    }
}