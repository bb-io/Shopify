using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Theme;

public class ThemeEntity
{
    [Display("ID")]
    public string Id { get; set; }

    [Display("Name")]
    public string Name { get; set; }

    [Display("Role")]
    public string Role { get; set; }

    [Display("Created at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }
}