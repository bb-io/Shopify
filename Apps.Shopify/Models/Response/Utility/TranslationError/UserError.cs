namespace Apps.Shopify.Models.Response.Utility.TranslationError;

public class UserError
{
    public List<string>? Field { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public override string ToString() => $"{string.Join('.', Field ?? [])}: {Message}";
}