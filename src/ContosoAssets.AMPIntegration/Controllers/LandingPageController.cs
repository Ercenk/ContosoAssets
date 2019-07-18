using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContosoAssets.AMPIntegration.Models;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using ContosoAssets.SolutionManagement.CustomerManagement;
using ContosoAssets.SolutionManagement.Data;
using ContosoAssets.SolutionManagement.Provisioning;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using ContosoAssets.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoAssets.AMPIntegration.Controllers
{
    [Authorize]
    public class LandingPageController : Controller
    {
        private readonly ICustomerManager customerManager;
        private readonly IFulfillmentManager fulfillmentManager;
        private readonly ILogger<LandingPageController> logger;
        private readonly IProvisioningManager provisioningManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ISubscriptionManager subscriptionManager;
        private readonly UserManager<IdentityUser> userManager;

        public LandingPageController(
            ISubscriptionManager subscriptionManager,
            IFulfillmentManager fulfillmentManager,
            IProvisioningManager provisioningManager,
            ICustomerManager customerManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<LandingPageController> logger)
        {
            this.subscriptionManager = subscriptionManager;
            this.fulfillmentManager = fulfillmentManager;
            this.provisioningManager = provisioningManager;
            this.customerManager = customerManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        // POST: LandingPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AzureSubscriptionProvisionModel provisionModel)
        {
            try
            {
                // First create the admin user on Contoso side
                var user = new IdentityUser {UserName = provisionModel.Email, Email = provisionModel.Email};

                var result = await this.userManager.CreateAsync(user, provisionModel.Password);

                // Create the user and admin roles if they are not already in the store
                await this.roleManager.EnsureRolesExistAsync();

                var roleResult = await this.userManager.AddToRoleAsync(user, Constants.CustomerAdminRoleName);

                // Better do this in a compensating transaction fashion, but this is a sample.
                // Implement a IDisposable transaction manager, register operations that be rolled back
                // as add, and roll back if fails in Dispose
                if (!await this.customerManager.CustomerUserExistsByUserNameAsync(provisionModel.Email))
                {
                    var addSubscriptionResult = await this.subscriptionManager.ProvisionSubscriptionWithAdminAsync(
                        provisionModel.Email,
                        provisionModel.CustomerRegion,
                        provisionModel.FullName,
                        provisionModel.PlanId.Value,
                        provisionModel.SubscriptionId);
                    foreach (var error in addSubscriptionResult.Errors)
                    {
                        this.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                var provisioningStatus = await this.provisioningManager.AddEnvironmentAsync(
                    provisionModel.Email.GetDomainNameFromEmail(),
                    provisionModel.PlanId.Value,
                    Enum.GetName(typeof(CustomerRegionEnum),
                        provisionModel.CustomerRegion));

                if (provisioningStatus.Success)
                {
                    var activationResult = await this.fulfillmentManager.ActivateSubscriptionAsync(
                        provisionModel.SubscriptionId,
                        provisionModel.PlanName,
                        null);

                    if (activationResult != default(MarketplaceSubscription))
                    {
                        activationResult.OfferId = provisionModel.OfferId;
                        activationResult.SubscriptionName = provisionModel.SubscriptionName;
                        return this.RedirectToAction(nameof(this.Success), activationResult);
                    }
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return this.View(ex);
            }
        }

        // GET: LandingPage
        public async Task<ActionResult> Index(string token)
        {
            var resolvedSubscription = await this.fulfillmentManager.ResolveSubscriptionAsync(token);
            if (resolvedSubscription == default(MarketplaceSubscription))
            {
                this.ModelState.AddModelError(string.Empty, "Cannot resolve subscription");
                return this.View();
            }

            var sku = (await this.subscriptionManager.GetSkusAsync(SalesChannelEnum.Azure))
                .FirstOrDefault(s => s.Name == resolvedSubscription.PlanId);

            if (sku == default(Sku))
            {
                return this.NotFound("A Sku with the plan name on the resolved subscription does not exist.");
            }

            var fullName = (this.User.Identity as ClaimsIdentity)?.FindFirst("name")?.Value;
            var emailAddress = (this.User.Identity as ClaimsIdentity)?.FindFirst("preferred_username")?.Value;

            var provisioningModel = new AzureSubscriptionProvisionModel
            {
                FullName = fullName,
                PlanName = resolvedSubscription.PlanId,
                SubscriptionId = resolvedSubscription.SubscriptionId,
                Email = emailAddress,
                PlanId = sku.Id,
                OfferId = resolvedSubscription.OfferId,
                SubscriptionName = resolvedSubscription.SubscriptionName
            };

            return this.View(provisioningModel);
        }

        public ActionResult Success(MarketplaceSubscription marketplaceSubscription)
        {
            return this.View(marketplaceSubscription);
        }
    }
}
