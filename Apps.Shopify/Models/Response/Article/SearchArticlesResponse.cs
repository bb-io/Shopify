namespace Apps.Shopify.Models.Response.Article;

public record SearchArticlesResponse(IEnumerable<GetArticleResponse> Articles);