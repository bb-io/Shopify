namespace Apps.Shopify.Models.Filters;

public interface ICreatedDateFilter : IDateFilter
{
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}
