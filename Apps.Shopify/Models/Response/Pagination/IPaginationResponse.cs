namespace Apps.Shopify.Models.Response.Pagination;

public interface IPaginationResponse<T>
{
    public PaginationData<T> Items { get; set; }
}