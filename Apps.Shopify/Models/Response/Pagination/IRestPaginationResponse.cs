namespace Apps.Shopify.Models.Response.Pagination;

public interface IRestPaginationResponse<T> where T : IRestPaginationEntity
{
    public IEnumerable<T> Items { get; set; }
}