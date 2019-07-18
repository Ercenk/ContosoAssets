using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using ContosoAssets.SolutionManagement.CustomerManagement;
using ContosoAssets.SolutionManagement.Provisioning;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using ContosoAssets.Utils;
using ContosoAssets.WebApp.Areas.SubscriptionManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoAssets.WebApp.Areas.SubscriptionManagement.Controllers
{
    [Area("SubscriptionManagement")]
    [Authorize(Roles = Constants.CustomerAdminRoleName,
        AuthenticationSchemes = Constants.IdentityApplicationSchemeName)]
    public class SubscriptionController : Controller
    {
        private readonly ILogger<SubscriptionController> logger;
        private readonly IProvisioningManager provisioningManager;
        private readonly ISubscriptionManager subscriptionManager;

        public SubscriptionController(
            ISubscriptionManager subscriptionManager,
            IProvisioningManager provisioningManager,
            ILogger<SubscriptionController> logger)
        {
            this.subscriptionManager = subscriptionManager;
            this.provisioningManager = provisioningManager;
            this.logger = logger;
        }

        public ICustomerManager CustomerUsersManager { get; }
        public ILogger Logger { get; }
        public ISubscriptionManager SkuManager { get; }
        public UserManager<IdentityUser> UserManager { get; }

        [HttpPost]
        public virtual async Task<IActionResult> CancelSubscription()
        {
            var cancelResult = await this.subscriptionManager.CancelSubscriptionAsync(this.User.Identity.Name);

            if (!cancelResult.Succeeded)
            {
                return this.ErrorAndRedirectHome("Cannot cancel subscription");
            }

            // We also have the option for waiting for the webhook to get notified that the delet operation initiated completed.
            // However, that means to haging on to the allocated resources longer than necessary. It is completely dependent
            // on the business case though. We are assuming removing the resourecs as soon as possible is the best case.
            var provisioningStatus =
                await this.provisioningManager.RemoveEnvironmentAsync(this.User.Identity.Name.GetDomainNameFromEmail());

            return this.Redirect("/");
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit()
        {
            var subscription = await this.subscriptionManager.GetSubscriptionAsync(this.User.Identity.Name);
            var currentSku = subscription.Sku;

            var skusForCustomerSkuChannel = await this.subscriptionManager.GetSkusAsync(currentSku.SalesChannel);

            var usages = (await this.subscriptionManager
                    .GetUsage(a => a.CustomerUser.CustomerName == this.User.Identity.Name.GetDomainNameFromEmail()))
                .ToList();

            var now = DateTimeOffset.UtcNow;
            var totalUsage = usages.Count();
            var monthToDateUsage = usages.Count(u => u.Timestamp >=
                                                     new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0,
                                                         TimeSpan.Zero));

            var skuModel = new SubscriptionModel
            {
                AvailableSkus = skusForCustomerSkuChannel,
                Subscription = subscription,
                CurrentSku = currentSku,
                TotalUsage = totalUsage,
                MonthToDateUsage = monthToDateUsage
            };

            return this.View(skuModel);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(SubscriptionModel skuModel)
        {
            var updateResult =
                await this.subscriptionManager.UpdateSubscriptionWithAdminNameAsync(this.User.Identity.Name,
                    skuModel.CurrentSku.Id);

            if (!updateResult.Succeeded)
            {
                return this.ErrorAndRedirectHome("Cannot cancel subscription");
            }

            return this.Redirect("/");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReActivateSubsription()
        {
            var subscription = await this.subscriptionManager.GetSubscriptionAsync(this.User.Identity.Name);
            subscription.State = SubscriptionState.Complete;

            var activateResult = await this.subscriptionManager.ReactivateSubscriptionAsync(subscription);

            if (!activateResult.Succeeded)
            {
                return this.ErrorAndRedirectHome("Cannot activate subscription");
            }

            return this.Redirect("/");
        }

        private IActionResult ErrorAndRedirectHome(string message)
        {
            this.ModelState.AddModelError("SubscriptionManagement", message);
            return this.Redirect("/");
        }
    }
}
