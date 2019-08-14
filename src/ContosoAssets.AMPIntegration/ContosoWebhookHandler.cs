using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaaSFulfillmentClient.WebHook;

namespace ContosoAssets.AMPIntegration
{
    using System;
    using System.Threading;

    using SaaSFulfillmentClient;
    using SaaSFulfillmentClient.Models;

    public class ContosoWebhookHandler : IWebhookHandler
    {
        private readonly IFulfillmentClient fulfillmentClient;
        private readonly ILogger<ContosoWebhookHandler> logger;
        private readonly ISubscriptionManager subscriptionManager;

        public ContosoWebhookHandler(ISubscriptionManager subscriptionManager, ILogger<ContosoWebhookHandler> logger, IFulfillmentClient fulfillmentClient)
        {
            this.subscriptionManager = subscriptionManager;
            this.logger = logger;
            this.fulfillmentClient = fulfillmentClient;
        }

        // We are handling only successful operation results for this sample. There can be scenarios where
        // other return types also need to be processed.

        public async Task ChangePlanAsync(WebhookPayload payload)
        {
            if (payload.Status == SaaSFulfillmentClient.Models.OperationStatusEnum.Succeeded)
            {
                var updateResult =
                    await this.subscriptionManager.UpdateSubscriptionAsync(payload.SubscriptionId,
                        payload.PlanId);

                if (updateResult.Succeeded)
                {
                    this.logger.LogInformation(
                        $"Subscription {payload.SubscriptionId} successfully updated to {payload.PlanId}");

                    await this.fulfillmentClient.UpdateSubscriptionOperationAsync(
                        payload.SubscriptionId,
                        payload.OperationId,
                        new OperationUpdate()
                        {
                            PlanId = payload.PlanId,
                            Quantity = payload.Quantity,
                            Status = OperationUpdateStatusEnum.Success
                        },
                        Guid.Empty,
                        Guid.Empty,
                        CancellationToken.None);
                }
                else
                {
                    this.logger.LogError(
                        $"Cannot update subscription {payload.SubscriptionId}  to {payload.PlanId}. Error details are {updateResult}");
                }
            }
        }

        public async Task ChangeQuantityAsync(WebhookPayload payload)
        {
            if (payload.Status == SaaSFulfillmentClient.Models.OperationStatusEnum.Succeeded)
            {
                // The sample does not have seat based skus. Not implementing this for now
                this.logger.LogInformation("Current implementation does not include a seat based SKU.");
                await Task.CompletedTask;
            }
        }

        public async Task ReinstatedAsync(WebhookPayload payload)
        {
            if (payload.Status == SaaSFulfillmentClient.Models.OperationStatusEnum.Succeeded)
            {
                var reinsteateResult = await this.subscriptionManager.ReactivateSubscriptionAsync(payload.SubscriptionId);
                if (reinsteateResult.Succeeded)
                {
                    await this.fulfillmentClient.UpdateSubscriptionOperationAsync(
                        payload.SubscriptionId,
                        payload.OperationId,
                        new OperationUpdate()
                        {
                            PlanId = payload.PlanId,
                            Quantity = payload.Quantity,
                            Status = OperationUpdateStatusEnum.Success
                        },
                        Guid.Empty,
                        Guid.Empty,
                        CancellationToken.None);

                    this.logger.LogInformation(
                        $"Subscription {payload.SubscriptionId} successfully reinstated.");
                }
                else
                {
                    this.logger.LogError(
                        $"Cannot reinstate subscription {payload.SubscriptionId}. Error details are {reinsteateResult}");
                }
            }
        }

        public async Task SubscribedAsync(WebhookPayload payload)
        {
            if (payload.Status == SaaSFulfillmentClient.Models.OperationStatusEnum.Succeeded)
            {
                // We are handling the subscription resolve and activate on the landing page for this sample.
                // Just log the result, since we already assumed the subscription started to get billed.
                // This can be a location where the user is notified, or some other system for keeping track of
                // the subscription status is updated.
                this.logger.LogInformation($"Activation succeeded for {JsonConvert.SerializeObject(payload)}");
                await Task.CompletedTask;
            }
        }

        public async Task SuspendedAsync(WebhookPayload payload)
        {
            if (payload.Status == SaaSFulfillmentClient.Models.OperationStatusEnum.Succeeded)
            {
                var suspendResult = await this.subscriptionManager.SuspendSubscriptionAsync(payload.SubscriptionId);
                if (suspendResult.Succeeded)
                {
                    await this.fulfillmentClient.UpdateSubscriptionOperationAsync(
                        payload.SubscriptionId,
                        payload.OperationId,
                        new OperationUpdate()
                        {
                            PlanId = payload.PlanId,
                            Quantity = payload.Quantity,
                            Status = OperationUpdateStatusEnum.Success
                        },
                        Guid.Empty,
                        Guid.Empty,
                        CancellationToken.None);

                    this.logger.LogInformation(
                        $"Subscription {payload.SubscriptionId} successfully suspended.");
                }
                else
                {
                    this.logger.LogError(
                        $"Cannot suspend subscription {payload.SubscriptionId}. Error details are {suspendResult}");
                }
            }
        }

        public async Task UnsubscribedAsync(WebhookPayload payload)
        {
            if (payload.Status == SaaSFulfillmentClient.Models.OperationStatusEnum.Succeeded)
            {
                // We are deprovisioning the customer resources the minute the admin user cancels the subscription
                // in the subscription management page. There can be multiple implementation options. The publisher may chose
                // to deallocate resources after the cancel is processed. Or archive things for the customer. We are assuming
                // all of that is handled right after cancel is requested. Just logging here

                // TODO: check how I can handle this if cancel is requested from the Azure side
                this.logger.LogInformation($"Cancel succeeded for {JsonConvert.SerializeObject(payload)}");
                await Task.CompletedTask;
            }
        }
    }
}
