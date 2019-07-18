using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using ContosoAssets.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ContosoAssets.WebApp.Controllers
{
    public class MeteredController : Controller
    {
        private readonly ISubscriptionManager skuManager;

        public MeteredController(ISubscriptionManager skuManager)
        {
            this.skuManager = skuManager;
        }

        protected async Task<bool> CheckLimitAndMeter([CallerMemberName] string memberName = "")
        {
            if (!await this.skuManager.IsUsageInSkuLimitsAsync(this.User.Identity.Name.GetDomainNameFromEmail()))
            {
                return false;
            }

            await this.skuManager.AddUsageAsync(
                this.User.Identity.Name,
                this.User.Identity.Name.GetDomainNameFromEmail(),
                this.GetType(),
                memberName);
            return true;

        }
    }
}
