namespace Apps.Shopify.Services.Models;

public class UploadContentServiceRequest
{
    public required string HtmlContent { get; set; }
    
    public required string Locale { get; set; }

    public string? MarketId { get; set; }
    
    public string? ContentId { get; set; }
}