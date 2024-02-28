namespace Apps.Shopify.Models.Request.Product;

public class ProductContentRequest
{
    public string Key { get; set; }
    
    public string Value { get; set; }
    
    public string Locale { get; set; }
    
    public string TranslatableContentDigest { get; set; }
}