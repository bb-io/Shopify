using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class MetafieldOwnerTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return [
            new DataSourceItem("PRODUCT", "Product"),
            new DataSourceItem("COLLECTION", "Collection"),
            new DataSourceItem("PAGE", "Page"),
            new DataSourceItem("CUSTOMER", "Customer"),
            new DataSourceItem("ORDER", "Order"),
            new DataSourceItem("ARTICLE", "Article (Blog Post)"),
            new DataSourceItem("BLOG", "Blog")
        ];
    }
}
