using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models;
using Apps.Shopify.Models.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList]
public class GenericActions : TranslatableResourceActions
{
    private readonly InvocationContext _invocationContext;
    private readonly IFileManagementClient _fileManagementClient;
    private readonly Dictionary<string, Func<NonPrimaryLocaleRequest, FileRequest, Task>> _contentUpdateActions;

    public GenericActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext, fileManagementClient)
    {
        _invocationContext = invocationContext;
        _fileManagementClient = fileManagementClient;
        _contentUpdateActions = new()
        {
            { HtmlContentTypes.OnlineStoreArticle, UpdateOnlineStoreArticle },
            { HtmlContentTypes.Collection, UpdateCollection },
            { HtmlContentTypes.MetafieldContent, UpdateMetafieldContent },
            { HtmlContentTypes.OnlineStoreBlogContent, UpdateOnlineStoreBlog },
            { HtmlContentTypes.OnlineStorePageContent, UpdateOnlineStorePage },
            { HtmlContentTypes.OnlineStoreThemeContent, UpdateOnlineStoreTheme },
            { HtmlContentTypes.ProductContent, UpdateProduct },
            { HtmlContentTypes.StoreResourcesContent, UpdateStoreResources },
            { HtmlContentTypes.StoreContent, UpdateStore }
        };
    }

    [Action("Upload content", Description = "Update the content from a (translated) file")]
    public async Task UpdateContent([ActionParameter] UpdateContentRequest request,
        [ActionParameter] NonPrimaryLocaleRequest locale, 
        [ActionParameter] FileRequest file)
    {
        var html = await GetHtmlFromFile(file.File);
        var contentType = request.ContentType ?? ShopifyHtmlConverter.ExtractContentTypeFromHtml(html) ?? throw new PluginMisconfigurationException("Content type does not exist in the HTML file and must be provided in the optional input");
        if (!_contentUpdateActions.TryGetValue(contentType, out var action))
        {
            throw new PluginMisconfigurationException($"Content type '{contentType}' is not supported for updating content.");
        }
        
        await action(locale, file);
    }
    
    private async Task UpdateOnlineStoreArticle(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var onlineStoreArticleActions = new OnlineStoreArticleActions(_invocationContext, _fileManagementClient);
        await onlineStoreArticleActions.UpdateOnlineStoreArticleContent(null, locale, file);
    }
    
    private async Task UpdateCollection(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var collectionActions = new CollectionActions(_invocationContext, _fileManagementClient);
        await collectionActions.UpdateCollectionContent(null, locale, file);
    }
    
    private async Task UpdateMetafieldContent(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var metafieldActions = new MetafieldActions(_invocationContext, _fileManagementClient);
        await metafieldActions.UpdateMetaFieldContent(locale, file);
    }
    
    private async Task UpdateOnlineStoreBlog(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var onlineStoreBlogActions = new OnlineStoreBlogActions(_invocationContext, _fileManagementClient);
        await onlineStoreBlogActions.UpdateOnlineStoreBlogContent(locale, file);
    }
    
    private async Task UpdateOnlineStorePage(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var onlineStorePageActions = new OnlineStorePageActions(_invocationContext, _fileManagementClient);
        await onlineStorePageActions.UpdateOnlineStorePageContent(null, locale, file);
    }
    
    private async Task UpdateOnlineStoreTheme(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var onlineStoreThemeActions = new OnlineStoreThemeActions(_invocationContext, _fileManagementClient);
        await onlineStoreThemeActions.UpdateOnlineStoreThemeContent(null, locale, file);
    }
    
    private async Task UpdateProduct(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        var productActions = new ProductActions(_invocationContext, _fileManagementClient);
        await productActions.UpdateProductContent(locale, file);
    }
    
    private async Task UpdateStoreResources(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        if (string.IsNullOrEmpty(locale.Locale))
        {
            throw new PluginMisconfigurationException("Locale is required for updating store resources content");
        }
        
        var storeResourcesActions = new StoreActions(_invocationContext, _fileManagementClient);
        await storeResourcesActions.UpdateStoreResourcesContent(new LocaleRequest { Locale = locale.Locale }, file);
    }
    
    private async Task UpdateStore(NonPrimaryLocaleRequest locale, FileRequest file)
    {
        if (string.IsNullOrEmpty(locale.Locale))
        {
            throw new PluginMisconfigurationException("Locale is required for updating store content");
        }
        
        var storeActions = new StoreActions(_invocationContext, _fileManagementClient);
        await storeActions.UpdateStoreContent(new LocaleRequest { Locale = locale.Locale }, file);
    }
}