using System;
using System.ComponentModel.DataAnnotations;
using SaaSFulfillmentClient.Models;

namespace ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment
{
    public class MarketplaceSubscription
    {
        public string OfferId { get; set; }
        public string PlanId { get; set; }
        public int Quantity { get; set; }
        public SubscriptionState State { get; set; }
        public Guid SubscriptionId { get; set; }

        [Display(Name = "Name")] public string SubscriptionName { get; set; }

        internal static MarketplaceSubscription From(ResolvedSubscription subscription, SubscriptionState newState)
        {
            return new MarketplaceSubscription
            {
                SubscriptionId = subscription.SubscriptionId,
                OfferId = subscription.OfferId,
                PlanId = subscription.PlanId,
                Quantity = subscription.Quantity,
                SubscriptionName = subscription.SubscriptionName,
                State = newState
            };
        }
    }
}
