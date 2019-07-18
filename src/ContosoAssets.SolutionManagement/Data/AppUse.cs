using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoAssets.SolutionManagement.Data
{
    public class AppUse
    {
        public string Company { get; internal set; }

        public CustomerUser CustomerUser { get; internal set; }

        public Guid CustomerUserId { get; set; }

        [Key] public Guid Id { get; internal set; }

        public string Operation { get; set; }

        public DateTimeOffset Timestamp { get; internal set; }
    }
}
