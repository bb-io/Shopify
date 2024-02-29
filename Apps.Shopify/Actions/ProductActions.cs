using System.Net.Mime;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.Product;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Product;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions;

public class ProductActions : ShopifyInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Search products", Description = "Search for products based on provided criterias")]
    public async Task<ListProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest input)
    {
        var variables = new Dictionary<string, object>();

        var query = new List<string>();
        if (input.Status is not null)
        {
            query.Add($"status:{input.Status}");
        }

        if (!string.IsNullOrWhiteSpace(query.ToString()))
        {
            variables["query"] = string.Join(" AND ", query);
        }

        var response = await Client
            .Paginate<ProductEntity, ProductsPaginationResponse>(GraphQlQueries.Products, variables);

        return new(response);
    }

    [Action("Get product content as HTML", Description = "Get content of a specific product in HTML format")]
    public async Task<FileResponse> GetProductContent([ActionParameter] ProductRequest input)
    {
        var response = await GetProductSourceContent(input.ProductId);
        var html = ShopifyHtmlConverter.ToHtml(response.TranslatableResource.TranslatableContent);

        return new()
        {
            File = await _fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{input.ProductId}.html")
        };
    }

    [Action("Get product translation content as HTML",
        Description = "Get content of a specific product translation in HTML format")]
    public async Task<FileResponse> GetProductTranslationContent([ActionParameter] ProductTranslationRequest input)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.ProductTranslationContent,
            Variables = new
            {
                resourceId = input.ProductId,
                locale = input.Locale
            }
        };
        var response = await Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
        var html = ShopifyHtmlConverter.ToHtml(response.TranslatableResource.Translations);

        return new()
        {
            File = await _fileManagementClient.UploadAsync(html, MediaTypeNames.Text.Html, $"{input.ProductId}.html")
        };
    }

    [Action("Update product content from HTML", Description = "Update content of a specific product from HTML file")]
    public async Task UpdateProductContent([ActionParameter] UpdateProductContentRequest input)
    {
        var file = await _fileManagementClient.DownloadAsync(input.File);
        var translations = ShopifyHtmlConverter.ToJson(file, input.Locale).ToArray();

        if (translations.Any(x => string.IsNullOrWhiteSpace(x.TranslatableContentDigest)))
        {
            var sourceContent = await GetProductSourceContent(input.ProductId);
            sourceContent.TranslatableResource.TranslatableContent.ToList()
                .ForEach(x => translations.First(y => y.Key == x.Key).TranslatableContentDigest = x.Digest);
        }

        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.TranslationsRegister,
            Variables = new
            {
                resourceId = input.ProductId, translations
            }
        };

        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<TranslatableResourceResponse> GetProductSourceContent(string productId)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.ProductContent,
            Variables = new
            {
                resourceId = productId
            }
        };
        return Client.ExecuteWithErrorHandling<TranslatableResourceResponse>(request);
    }
}