namespace Apps.Shopify.Models.Response;

public class PaginationResponse<T>
{
    public IEnumerable<T> Items { get; set; }
}