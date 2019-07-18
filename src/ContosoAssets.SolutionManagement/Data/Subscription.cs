using System;
using System.ComponentModel.DataAnnotations;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;

namespace ContosoAssets.SolutionManagement.Data
{
    public class Subscription : MarketplaceSubscription
    {
        [Display(Name = "Created at")] public DateTimeOffset CreatedTime { get; set; }

        public Customer Customer { get; set; }

        public Guid CustomerId { get; set; }

        [Display(Name = "Last modified at")] public DateTimeOffset LastOperationTime { get; set; }

        public Sku Sku { get; set; }

        public Guid SkuId { get; set; }
    }
}
