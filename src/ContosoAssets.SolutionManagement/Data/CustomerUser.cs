using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContosoAssets.SolutionManagement.Data
{
    public class CustomerUser
    {
        [DisplayName("Created date")] public virtual DateTimeOffset CreatedDate { get; set; }

        public Customer Customer { get; internal set; }

        public Guid CustomerId { get; set; }

        [DisplayName("Customer")] public virtual string CustomerName { get; set; }

        [DisplayName("Customer region")] public CustomerRegionEnum? CustomerRegion { get; set; }

        public string ExternalUserName { get; internal set; }

        [DisplayName("Full name")] public string FullName { get; set; }

        [Key] public virtual Guid Id { get; set; }

        public List<AppUse> Usages { get; set; }

        [DisplayName("User name")] public virtual string UserName { get; set; }
    }
}
