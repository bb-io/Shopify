using Apps.Shopify.Constants;
using Apps.Shopify.Services.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services.Concrete;

public class StoreThemeSettingsDataSectionService(InvocationContext invocationContext) 
    : BaseTranslatableResourceContentService(invocationContext)
{
    protected override string ContentType => TranslatableResources.StoreThemeSettingsDataSection;
}