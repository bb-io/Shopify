using System.Net.Mime;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
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

    protected async Task<FileResponse> GetResourceContent(string resourceId, string locale)
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
        var html = ShopifyHtmlConverter.ToHtml(response.TranslatableResource.Translations.Any()
            ? response.TranslatableResource.Translations
            : response.TranslatableResource.TranslatableContent);

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{resourceId.GetShopifyItemId()}.html")
        };
    }

    protected async Task UpdateResourceContent(string resourceId, string locale, FileReference file)
    {
        var fileStream = await FileManagementClient.DownloadAsync(file);
        var translations = ShopifyHtmlConverter.ToJson(fileStream, locale).ToList();

        if (translations.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
        {
            var sourceContent = await GetResourceSourceContent(resourceId);
            translations.ForEach(x =>
                x.TranslatableContentDigest = sourceContent.TranslatableResource.TranslatableContent
                    .First(y => y.Key == x.Key).Digest);
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

    protected async Task<ICollection<TranslatableResourceEntity>> ListTranslatableResources(TranslatableResource resourceType, string? locale = default)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceType"] = resourceType,
            ["locale"] = locale ?? string.Empty
        };
        return await Client
            .Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
                GraphQlQueries.TranslatableResourcesWithTranslations,
                variables, default);
    }

    protected Task<TranslatableResourceResponse> GetResourceSourceContent(string resourceId)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.TranslatableResourceContent,
            Variables = new
            {
                resourceId
            }
        };
        return Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
    }
}