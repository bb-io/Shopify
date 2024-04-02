using System.Net.Mime;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions.Base;

public class TranslatableResourceActions : ShopifyInvocable
{
    protected readonly IFileManagementClient FileManagementClient;

    protected TranslatableResourceActions(InvocationContext invocationContext,
        IFileManagementClient fileManagementClient)
        : base(invocationContext)
    {
        FileManagementClient = fileManagementClient;
    }

    protected async Task<FileResponse> GetResourceContent(string resourceId)
    {
        var response = await GetResourceSourceContent(resourceId);
        var html = ShopifyHtmlConverter.ToHtml(response.TranslatableResource.TranslatableContent);

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{resourceId}.html")
        };
    }
    
    protected async Task<FileResponse> GetResourceTranslationContent(string resourceId, string locale)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceTranslations,
            Variables = new
            {
                resourceId, locale
            }
        };
        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        var html = ShopifyHtmlConverter.ToHtml(response.TranslatableResource.Translations);

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{resourceId}.html")
        };
    }

    protected async Task UpdateResourceContent(string resourceId, string locale, FileReference file)
    {
        var fileStream = await FileManagementClient.DownloadAsync(file);
        var translations = ShopifyHtmlConverter.ToJson(fileStream, locale).ToArray();

        if (translations.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
        {
            var sourceContent = await GetResourceSourceContent(resourceId);
            sourceContent.TranslatableResource.TranslatableContent.ToList()
                .ForEach(x => translations.First(y => y.Key == x.Key).TranslatableContentDigest = x.Digest);
        }

        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.TranslationsRegister,
            Variables = new
            {
                resourceId,
                translations
            }
        };

        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<TranslatableResourceResponse> GetResourceSourceContent(string resourceId)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceContent,
            Variables = new
            {
                resourceId = resourceId
            }
        };
        return Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
    }
}