namespace Apps.Shopify.HtmlConversion.Models;

public record TextRange(int Start, int Length)
{
    public int End => Start + Length;
}