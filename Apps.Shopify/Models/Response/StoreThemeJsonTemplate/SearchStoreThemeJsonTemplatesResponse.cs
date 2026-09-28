using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeJsonTemplate;

public record SearchStoreThemeJsonTemplatesResponse(List<StoreThemeJsonTemplateResponse> Templates)
{
    [Display("Store theme JSON templates")]
    public List<StoreThemeJsonTemplateResponse> Templates { get; set; } = Templates;
}