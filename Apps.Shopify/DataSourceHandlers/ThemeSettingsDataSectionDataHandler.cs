using Apps.Shopify.DataSourceHandlers.TranslatableResourceBase;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ThemeSettingsDataSectionDataHandler(InvocationContext invocationContext) 
    : TranslatableResourceDataHandler(invocationContext)
{
    protected override TranslatableResource ResourceType => TranslatableResource.ONLINE_STORE_THEME_SETTINGS_DATA_SECTIONS;
}