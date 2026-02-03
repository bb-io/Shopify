namespace Apps.Shopify.Models.Filters;

public interface IUpdatedDateFilter : IDateFilter
{
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
}
