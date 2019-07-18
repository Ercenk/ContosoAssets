﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SaaSFulfillmentClient;
using SaaSFulfillmentClient.Models;

namespace ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment
{
    public class FulfillmentManager : IFulfillmentManager
    {
        private readonly IFulfillmentClient fulfillmentClient;
        private readonly ILogger<FulfillmentManager> logger;
        private readonly ICredentialProvider credentialProvider;

        private readonly FulfillmentManagerOptions options;

        public FulfillmentManager(
            IOptionsMonitor<FulfillmentManagerOptions> optionsAccessor,
            ICredentialProvider credentialProvider,
            IFulfillmentClient fulfillmentClient,
            ILogger<FulfillmentManager> logger) : this(
            optionsAccessor,
            credentialProvider,
            fulfillmentClient,
            AdApplicationHelper.GetApplication,
            logger)
        {
        }

        public FulfillmentManager(
            IOptionsMonitor<FulfillmentManagerOptions> optionsAccessor,
            ICredentialProvider credentialProvider,
            IFulfillmentClient fulfillmentClient,
            Func<FulfillmentManagerOptions, ICredentialProvider, IConfidentialClientApplication> adApplicationFactory,
            ILogger<FulfillmentManager> logger)
        {
            this.options = optionsAccessor.CurrentValue;

            this.credentialProvider = credentialProvider;
            this.fulfillmentClient = fulfillmentClient;
            this.logger = logger;
            this.AdApplication = adApplicationFactory(this.options, this.credentialProvider);
        }

        public IConfidentialClientApplication AdApplication { get; }

        public async Task<MarketplaceSubscription> ActivateSubscriptionAsync(Guid subscriptionId, string planId,
            int? quantity, CancellationToken cancellationToken = default)
        {
            var bearerToken = await AdApplicationHelper.GetBearerToken(this.AdApplication);

            var requestId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var subscriptionToBeActivated = new ActivatedSubscription {PlanId = planId};
            if (quantity.HasValue)
            {
                subscriptionToBeActivated.Quantity = quantity.Value.ToString();
            }

            var result = await this.fulfillmentClient.ActivateSubscriptionAsync(
                subscriptionId,
                subscriptionToBeActivated,
                requestId,
                correlationId,
                bearerToken,
                cancellationToken);

            if (result.Success)
            {
                this.logger.LogInformation(
                    $"Activated subscription {subscriptionId} for plan {planId} with quantitiy {quantity}");
                var returnValue = new MarketplaceSubscription
                {
                    PlanId = planId, State = SubscriptionState.Complete, SubscriptionId = subscriptionId
                };

                if (quantity.HasValue)
                {
                    returnValue.Quantity = quantity.Value;
                }

                return returnValue;
            }

            this.logger.LogError(
                $"Cannot activate subscription {subscriptionId} for plan {planId} with quantitiy {quantity}");

            return default;
        }

        public async Task<FulfillmentManagerOperationResult> GetOperationResultAsync(Guid receivedSubscriptionId,
            Guid operationId, CancellationToken cancellationToken = default)
        {
            var requestId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var bearerToken = await AdApplicationHelper.GetBearerToken(this.AdApplication);

            var operationResult = await this.fulfillmentClient.GetSubscriptionOperationAsync(
                receivedSubscriptionId,
                operationId,
                requestId,
                correlationId,
                bearerToken,
                cancellationToken);

            if (!operationResult.Success)
            {
                return FulfillmentManagerOperationResult.Failed(
                    new FulfillmentManagementError
                    {
                        Description =
                            $"Check operation error. Subscription {receivedSubscriptionId}, operation {operationId}"
                    });
            }

            if (operationResult.Status == OperationStatusEnum.Succeeded)
            {
                return FulfillmentManagerOperationResult.Success;
            }

            if (operationResult.Status == OperationStatusEnum.Failed ||
                operationResult.Status == OperationStatusEnum.Conflict)
            {
                return FulfillmentManagerOperationResult.Failed(
                    new FulfillmentManagementError
                    {
                        Description =
                            $"Check operation error. Status is {operationResult.Status}. Subscription {receivedSubscriptionId}, operation {operationId}"
                    });
            }

            // Operation status does not return a retry-after header. We hardcoded for 10 seconds for now.
            return FulfillmentManagerOperationResult.Success;
        }

        public async Task<FulfillmentManagerOperationResult> RequestCancelSubscriptionAsync(Guid subscriptionId,
            CancellationToken cancellationToken = default)
        {
            var requestId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var bearerToken = await AdApplicationHelper.GetBearerToken(this.AdApplication);

            var deleteRequest = await this.fulfillmentClient.DeleteSubscriptionAsync(subscriptionId, requestId,
                correlationId, bearerToken, cancellationToken);

            if (!deleteRequest.Success)
            {
                return FulfillmentManagerOperationResult.Failed(new FulfillmentManagementError {Description = ""});
            }

            var operationUri = deleteRequest.Operation;
            Guid operationId;
            try
            {
                operationId = this.ExtractOperationId(subscriptionId, operationUri);
            }
            catch (FulfillmentManagerException exception)
            {
                return exception.OperationResult;
            }

            return FulfillmentManagerOperationResult.Success;
        }

        public async Task<FulfillmentManagerOperationResult> RequestUpdateSubscriptionAsync(Guid subscriptionId,
            string name, CancellationToken cancellationToken = default)
        {
            var requestId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var bearerToken = await AdApplicationHelper.GetBearerToken(this.AdApplication);

            var activatedSubscription = new ActivatedSubscription {PlanId = name};

            var updateResponse = await this.fulfillmentClient.UpdateSubscriptionAsync(subscriptionId,
                activatedSubscription, requestId, correlationId, bearerToken, cancellationToken);

            if (!updateResponse.Success)
            {
                return FulfillmentManagerOperationResult.Failed(new FulfillmentManagementError
                {
                    Description = $"Update request error. Subscription id {subscriptionId}"
                });
            }

            var operationUri = updateResponse.Operation;
            Guid operationId;
            try
            {
                operationId = this.ExtractOperationId(subscriptionId, operationUri);
            }
            catch (FulfillmentManagerException exception)
            {
                return exception.OperationResult;
            }

            return FulfillmentManagerOperationResult.Success;
        }

        public async Task<MarketplaceSubscription> ResolveSubscriptionAsync(string authCode,
            CancellationToken cancellationToken = default)
        {
            var bearerToken = await AdApplicationHelper.GetBearerToken(this.AdApplication);

            var requestId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var subscription = await this.fulfillmentClient.ResolveSubscriptionAsync(authCode, requestId, correlationId,
                bearerToken, cancellationToken);

            if (subscription.Success)
            {
                this.logger.LogInformation(
                    $"Resolved subscription {subscription.SubscriptionId} for plan {subscription.PlanId} with quantitiy {subscription.Quantity}");
                return MarketplaceSubscription.From(subscription, SubscriptionState.Pending);
            }

            this.logger.LogError("Cannot resolve subscription.");
            return default;
        }

        private Guid ExtractOperationId(Guid subscriptionId, Uri operationUri)
        {
            // We expect an operation URI like
            // https://marketplaceapi.microsoft.com/api/saas/subscriptions/37f9dea2-4345-438f-b0bd-03d40d28c7e0/operations/529f53e1-c04b-49c8-881c-c49fb5c6fada?api-version=2018-09-15
            var uriSegments = operationUri.Segments.Select(s => s.TrimEnd('/')).ToArray();

            if (uriSegments.Length != 7)
            {
                throw new FulfillmentManagerException(FulfillmentManagerOperationResult.Failed(
                    new FulfillmentManagementError
                    {
                        Description =
                            $"The received operation Uri is not valid. It does not have 7 segments: {operationUri}"
                    }));
            }

            if (!(Guid.TryParse(uriSegments[4], out var receivedSubscriptionId) ||
                  receivedSubscriptionId != subscriptionId))
            {
                throw new FulfillmentManagerException(FulfillmentManagerOperationResult.Failed(
                    new FulfillmentManagementError
                    {
                        Description =
                            $"The received subscription Id is not a valid Guid, or not equal to the original subscription. {uriSegments[6]}"
                    }));
            }

            if (!Guid.TryParse(uriSegments[6], out var operationId))
            {
                throw new FulfillmentManagerException(FulfillmentManagerOperationResult.Failed(
                    new FulfillmentManagementError
                    {
                        Description = $"The received operation Id is not a valid Guid. {uriSegments[6]}"
                    }));
            }

            return operationId;
        }
    }
}
