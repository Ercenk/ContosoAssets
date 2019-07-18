using System;
using System.ComponentModel;

namespace ContosoAssets.WebApp.Models
{
    public enum AssetStatusEnum
    {
        Out,
        In,
        Broken,
        OnRepair,
        Unknown
    }

    public class Asset
    {
        public string CustomerName { get; set; }
        public string Id { get; set; }

        [DisplayName("Last update time")] public DateTimeOffset LastUpdateTime { get; set; }

        public string Name { get; set; }
        public AssetStatusEnum Status { get; set; }
    }
}
