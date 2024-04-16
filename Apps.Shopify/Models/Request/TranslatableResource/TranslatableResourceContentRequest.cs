namespace Apps.Shopify.Models.Request.TranslatableResource;

public class TranslatableResourceContentRequest
{
    public string Key { get; set; }
    
    public string Value { get; set; }
    
    public string Locale { get; set; }
    
    public string TranslatableContentDigest { get; set; }
}