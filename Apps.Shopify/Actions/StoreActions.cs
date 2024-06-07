using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Response.Locale;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Actions
{
    [ActionList]
    public class StoreActions : TranslatableResourceActions
    {
        public StoreActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
        {
        }

        [Action("Get store locales information", Description = "Get primary and other locales")]
        public async Task<StoreLocalesResponse> GetStoreLanguages() 
        {
            var request = new GraphQLRequest()
            {
                Query = GraphQlQueries.Locales
            };
            var response = await Client.ExecuteWithErrorHandling<ShopLocalesResponse>(request);

            return new StoreLocalesResponse
            {
                Primary = response.ShopLocales.First(x => x.Primary),
                OtherLocales = response.ShopLocales.Where(x => x.Primary is false)
            };  
        }

    }
}
