using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.CustomerManagement;
using ContosoAssets.SolutionManagement.Data;
using ContosoAssets.SolutionManagement.Provisioning;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using ContosoAssets.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoAssets.WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerManager customerManager;
        private readonly string DefaultActivationCode = "activate";
        private readonly IProvisioningManager provisioningManager;
        private readonly ISubscriptionManager subscriptionManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ICustomerManager customerUsersManager,
            IProvisioningManager provisioningManager,
            ISubscriptionManager subscriptionManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this.customerManager = customerUsersManager;
            this.provisioningManager = provisioningManager;
            this.subscriptionManager = subscriptionManager;
            this._logger = logger;
            this._emailSender = emailSender;

            this.Skus = new List<Sku>();
        }

        [BindProperty] public SubscriptionProvisionModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<Sku> Skus { get; private set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
            this.Skus = await this.subscriptionManager.GetSkusAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("/");

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            if (this.Input.IsAdmin && !(this.Input.CustomerRegion.HasValue || this.Input.PlanId.HasValue))
            {
                this.ModelState.AddModelError("AdminRegitration", "Region and plan has to be valid for admin users.");
                this.Skus = await this.subscriptionManager.GetSkusAsync();
                return this.Page();
            }

            // Create the user and admin roles if they are not already in the store
            await this._roleManager.EnsureRolesExistAsync();

            // Hardcode the activation code. In a real scenario this should be retrieved
            // from a store and checked if it is valid.
            if (this.Input.IsAdmin && this.Input.ActivationCode != this.DefaultActivationCode ||
                !this.Input.IsAdmin && !await this.customerManager.CustomerUserExistsByUserNameAsync(this.Input.Email))
            {
                this.ModelState.AddModelError("UserRegistration",
                    "An administrator needs to add a user before he/she can register.");
                this.Skus = await this.subscriptionManager.GetSkusAsync();
                return this.Page();
            }

            var user = new IdentityUser {UserName = this.Input.Email, Email = this.Input.Email};
            var result = await this._userManager.CreateAsync(user, this.Input.Password);

            if (result.Succeeded)
            {
                this._logger.LogInformation("User created a new account with password.");

                var userRole = Constants.CustomerUserRoleName;

                if (this.Input.IsAdmin)
                {
                    userRole = Constants.CustomerAdminRoleName;

                    // Better do this in a compensating transaction fashion, but this is a sample.
                    // Implement a IDisposable transaction manager, register operations that be rolled back
                    // as add, and roll back if fails in Dispose
                    if (!await this.customerManager.CustomerUserExistsByUserNameAsync(user.UserName))
                    {
                        var addSubscriptionResult = await this.subscriptionManager.ProvisionSubscriptionWithAdminAsync(
                            user.UserName,
                            this.Input.CustomerRegion,
                            this.Input.FullName,
                            this.Input.PlanId.Value,
                            Guid.NewGuid());
                        foreach (var error in addSubscriptionResult.Errors)
                        {
                            this.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    var provisioningStatus = await this.provisioningManager.AddEnvironmentAsync(
                        this.User.Identity.Name.GetDomainNameFromEmail(),
                        this.Input.PlanId.Value,
                        Enum.GetName(typeof(CustomerRegionEnum),
                            this.Input.CustomerRegion));
                }
                else
                {
                    // If user, then the admin should have already created this user using the UI and we have already checked above.
                    // then just update the user with the full name
                    var customerUser = await this.customerManager.GetCustomerUserByUserNameAsync(user.UserName);
                    customerUser.FullName = this.Input.FullName;

                    await this.customerManager.UpdateCustomerUserAsync(customerUser);
                }

                var roleResult = await this._userManager.AddToRoleAsync(user, userRole);
                foreach (var error in roleResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = this.Url.Page(
                    "/Account/ConfirmEmail",
                    null,
                    new {userId = user.Id, code},
                    this.Request.Scheme);

                await this._emailSender.SendEmailAsync(this.Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                await this._signInManager.SignInAsync(user, false);
                return this.LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            this.Skus = await this.subscriptionManager.GetSkusAsync();
            return this.Page();
        }
    }
}
