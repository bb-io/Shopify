namespace Apps.Shopify.Models.Response.Pagination;

public class PageInfo
{
    public bool HasNextPage { get; set; }
    
    public string EndCursor { get; set; }
}