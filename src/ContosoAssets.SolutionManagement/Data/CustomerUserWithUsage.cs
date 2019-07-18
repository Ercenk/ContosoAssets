using System;
using System.ComponentModel;
using System.Linq;

namespace ContosoAssets.SolutionManagement.Data
{
    public class CustomerUserWithUsage : CustomerUser
    {
        public CustomerUserWithUsage()
        {
        }

        public CustomerUserWithUsage(CustomerUser cu)
        {
            this.UserName = cu.UserName;
            this.CreatedDate = cu.CreatedDate;
            this.CustomerName = cu.CustomerName;
            this.FullName = cu.FullName;
            this.Id = cu.Id;
            this.TotalUsage = cu.Usages?.Count ?? 0;

            var now = DateTimeOffset.UtcNow;

            this.MtdUsage = cu.Usages?.Count(u => u.Timestamp >=
                                                  new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero)) ??
                            0;
        }

        [DisplayName("Month to date usage")] public int MtdUsage { get; set; }

        [DisplayName("Total usage")] public int TotalUsage { get; set; }
    }
}
