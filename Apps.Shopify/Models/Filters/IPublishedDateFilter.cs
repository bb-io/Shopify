namespace Apps.Shopify.Models.Filters;

public interface IPublishedDateFilter : IDateFilter
{
    public DateTime? PublishedAfter { get; set; }
    public DateTime? PublishedBefore { get; set; }
}
