namespace Apps.Shopify.Models.Response.Pagination;

public class PaginationData<T>
{
    public IEnumerable<T> Nodes { get; set; }
    
    public PageInfo PageInfo { get; set; }
}