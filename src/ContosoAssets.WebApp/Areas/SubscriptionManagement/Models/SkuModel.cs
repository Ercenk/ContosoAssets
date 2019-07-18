using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContosoAssets.SolutionManagement.Data;

namespace ContosoAssets.WebApp.Areas.SubscriptionManagement.Models
{
    public class SubscriptionModel
    {
        [Display(Name = "Available Offers")] public IEnumerable<Sku> AvailableSkus { get; set; }

        public Sku CurrentSku { get; set; }

        [Display(Name = "Month to date usage")]
        public int MonthToDateUsage { get; set; }

        public Subscription Subscription { get; set; }

        [Display(Name = "Total usage")] public int TotalUsage { get; set; }
    }
}
