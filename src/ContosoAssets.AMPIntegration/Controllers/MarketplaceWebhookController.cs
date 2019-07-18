using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContosoAssets.AMPIntegration.Controllers
{
    public class MarketplaceWebhookController : Controller
    {
        private readonly ILogger logger;
        private readonly ISubscriptionManager subscriptionManager;
        private readonly IWebhookProcessor webhookProcessor;

        public MarketplaceWebhookController(IWebhookProcessor webhookProcessor, ISubscriptionManager subscriptionManager, ILogger logger)
        {
            this.webhookProcessor = webhookProcessor;
            this.subscriptionManager = subscriptionManager;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Notify(WebhookPayload payload)
        {
            await this.webhookProcessor.ProcessWebhookNotificationAsync(payload);

            return this.Ok();
        }
    }
}
