using System.Net.Mime;
using Apps.Shopify.Actions.Base;
using Apps.Shopify.Api;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Metafield;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions;

[ActionList]
public class MetafieldActions : TranslatableResourceActions
{
    public MetafieldActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext, fileManagementClient)
    {
    }

    [Action("Get metafield content as HTML",
        Description = "Get metafield content of a specific product in HTML format")]
    public async Task<FileResponse> GetMetafieldContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] LocaleRequest locale, [ActionParameter] GetContentRequest getContentRequest)
    {
        var productMetaFields = await GetProductMetafields(resourceRequest.ProductId);
        var metaFields = await ListTranslatableResources(TranslatableResource.METAFIELD, locale.Locale,
            getContentRequest.Outdated ?? default);

        var resources = metaFields
            .Where(x => productMetaFields.Any(y => x.ResourceId == y.Id))
            .ToArray();

        var contents = resources.All(x => !x.Translations.Any())
            ? resources.Select(x => (x.ResourceId, x.TranslatableContent.FirstOrDefault())).ToArray()
            : resources.Select(x => (x.ResourceId, x.Translations.FirstOrDefault())).ToArray();

        var html = ShopifyHtmlConverter.MetaFieldsToHtml(contents.Where(x => x.Item2 is not null));

        return new()
        {
            File = await FileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html,
                $"{resourceRequest.ProductId.GetShopifyItemId()}-metafields.html")
        };
    }

    [Action("Update metafield content from HTML",
        Description = "Update metafield content of a specific product from HTML file")]
    public async Task UpdateMetaFieldContent([ActionParameter] ProductRequest resourceRequest,
        [ActionParameter] NonPrimaryLocaleRequest locale, [ActionParameter] FileRequest file)
    {
        var fileStream = await FileManagementClient.DownloadAsync(file.File);
        var translations = ShopifyHtmlConverter.MetaFieldsToJson(fileStream, locale.Locale).ToList();

        await FillDigests(translations, resourceRequest.ProductId);

        foreach (var translatableMetaFieldContentRequest in translations)
        {
            try
            {
                await UpdateMetaFieldContent(translatableMetaFieldContentRequest);
            }
            catch (Exception ex)
            {
                InvocationContext.Logger?.LogError(ex.ToString(), null);
            }
        }
    }

    private async Task<ICollection<MetafieldEntity>> GetProductMetafields(string productId)
    {
        var variables = new Dictionary<string, object>()
        {
            ["resourceId"] = productId
        };

        var client = new ShopifyClient(Creds, ShopifyClient.GenerateApiUrl(Creds, "unstable"));
        return await client.Paginate<MetafieldEntity, MetafieldPaginationResponse>(GraphQlQueries.ProductMetaFields,
            variables);
    }

    private async Task UpdateMetaFieldContent(IdentifiedContentRequest contentRequest)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.TranslationsRegister,
            Variables = new
            {
                resourceId = contentRequest.ResourceId,
                translations = new List<TranslatableResourceContentRequest>()
                {
                    new()
                    {
                        Key = contentRequest.Key,
                        TranslatableContentDigest = contentRequest.TranslatableContentDigest,
                        Value = contentRequest.Value,
                        Locale = contentRequest.Locale
                    }
                }
            }
        };

        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task FillDigests(List<IdentifiedContentRequest> translations,
        string productId)
    {
        if (translations.All(x => !string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
            return;

        var metaFields = await GetProductMetafields(productId);

        translations.ForEach(x =>
            x.TranslatableContentDigest = metaFields.FirstOrDefault(y => y.Id == x.ResourceId)?.CompareDigest ??
                                          throw new ArgumentException(
                                              "Provided Product ID does not correspond to the metafields specified in the HTML file"));
    }
}