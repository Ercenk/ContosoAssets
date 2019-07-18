using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoAssets.SolutionManagement.Data
{
    public class Sku
    {
        public string Description { get; set; }

        [Key] public Guid Id { get; set; }

        public double MonthlyCost { get; set; }

        public int MonthlyLimit { get; internal set; }

        [Display(Name = "Subscription plan")] public string Name { get; set; }

        // NOTE: added for Azure Marketplace integartion
        public SalesChannelEnum SalesChannel { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}
