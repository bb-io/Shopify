namespace Apps.Shopify.Models.Entities;

public class EventEntity
{
    public string Id { get; set; }
    
    public string Topic { get; set; }
    
    public string CallbackUrl { get; set; }
}