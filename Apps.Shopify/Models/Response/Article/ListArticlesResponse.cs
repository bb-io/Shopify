using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Article
{
    public class ListArticlesResponse
    {
        public IEnumerable<Article> Articles { get; set; }
    }

    public class Article
    {
        [Display("Resource ID")]
        public string ResourceId { get; set; }

        public string Title { get; set; }
    }
}
