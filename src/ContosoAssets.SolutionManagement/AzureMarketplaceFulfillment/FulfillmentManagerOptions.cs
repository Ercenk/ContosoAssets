using SaaSFulfillmentClient.Models;

namespace ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment
{
    public class FulfillmentManagerOptions
    {
        public AuthenticationConfiguration AzureActiveDirectory { get; set; }

        public FulfillmentClientConfiguration FulfillmentService { get; set; }
    }
}
