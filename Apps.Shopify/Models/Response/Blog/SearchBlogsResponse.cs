using Apps.Shopify.Models.Entities.Blog;

namespace Apps.Shopify.Models.Response.Blog;

public record SearchBlogsResponse(IEnumerable<BlogEntity> Blogs);