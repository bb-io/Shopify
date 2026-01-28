using Apps.Shopify.Helper;
using Apps.Shopify.Models.Entities.Product;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Product;

public class GetProductResponse(ProductEntity entity)
{
    [Display("ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Title")]
    public string Title { get; set; } = entity.Title;

    [Display("Description")]
    public string? Description { get; set; } = string.IsNullOrEmpty(entity.Description) ? null : entity.Description;

    [Display("Handle")]
    public string Handle { get; set; } = entity.Handle;

    [Display("Status")]
    public string Status { get; set; } = entity.Status;

    [Display("Product type")]
    public string? ProductType { get; set; } = string.IsNullOrEmpty(entity.ProductType) ? null : entity.ProductType;

    [Display("Online store URL")]
    public string? OnlineStoreUrl { get; set; } = entity.OnlineStoreUrl;

    [Display("Created at")]
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;

    [Display("Published at")]
    public DateTime? PublishedAt { get; set; } = entity.PublishedAt;

    [Display("Metafield")]
    public string? Metafield { get; set; } = MetafieldFormatter.Prettify(entity.Metafield?.Value);
}
