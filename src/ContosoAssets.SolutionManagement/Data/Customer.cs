using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoAssets.SolutionManagement.Data
{
    public class Customer
    {
        public string CustomerName { get; set; }

        [Key] public Guid Id { get; set; }

        public List<Subscription> Subscriptions { get; internal set; }

        public List<CustomerUser> Users { get; set; }
    }
}
