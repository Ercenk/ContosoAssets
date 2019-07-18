using System;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.CustomerManagement;
using ContosoAssets.SolutionManagement.Data;
using ContosoAssets.Utils;
using ContosoAssets.WebApp.Areas.SubscriptionManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContosoAssets.WebApp.Areas.SubscriptionManagement.Controllers
{
    [Area("SubscriptionManagement")]
    // See https://github.com/aspnet/AspNetCore/blob/master/src/Identity/Core/src/IdentityConstants.cs#L15
    [Authorize(Roles = Constants.CustomerAdminRoleName, AuthenticationSchemes = "Identity.Application")]
    public class UsersController : Controller
    {
        private readonly ICustomerManager customerManagementStore;
        private readonly ILogger<UsersController> logger;
        private readonly IUserManagerAdapter userManager;

        public UsersController(ICustomerManager customerManagementStore, IUserManagerAdapter userManager,
            ILogger<UsersController> logger)
        {
            this.customerManagementStore = customerManagementStore;
            this.userManager = userManager;
            this.logger = logger;
        }

        // GET: CustomerUsers/Home/Create
        public IActionResult Create()
        {
            var createUserModel = new CustomerUserModel
            {
                CustomerName = this.User.Identity.Name.GetDomainNameFromEmail(), CustomerUserName = ""
            };
            return this.View(createUserModel);
        }

        // POST: CustomerUsers/Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CustomerUserModel user)
        {
            var userName = $"{user.CustomerUserName.Trim()}@{user.CustomerName.Trim()}";

            CustomerUser customerUser = null;
            if (this.ModelState.IsValid && userName.IsValidEmail())
            {
                // Only add user if it is the same domain name
                if (this.User.Identity.Name.GetDomainNameFromEmail()
                    == userName.GetDomainNameFromEmail())
                {
                    customerUser = new CustomerUser
                    {
                        UserName = userName,
                        CreatedDate = DateTimeOffset.UtcNow,
                        Id = Guid.NewGuid(),
                        CustomerName = this.User.Identity.Name.GetDomainNameFromEmail()
                    };

                    // Only the admin can land here, and admin's name is the email.
                    // We assume the customer name is  the email domain
                    _ = await this.customerManagementStore.AddUserAsync(customerUser);

                    return this.RedirectToAction(nameof(this.Index));
                }
            }

            this.ModelState.AddModelError("Username", "User name is not a valid email.");
            return this.View(customerUser);
        }

        // GET: CustomerUsers/Home/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var customerUser = await this.customerManagementStore.GetCustomerUserByIdAsync(id);

            if (customerUser == null)
            {
                return this.NotFound();
            }

            return this.View(customerUser);
        }

        // POST: CustomerUsers/Home/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customerUser = await this.customerManagementStore.GetCustomerUserByIdAsync(id);
            if (await this.customerManagementStore.DeleteCustomerUserAsync(customerUser) ==
                SolutionManagerOperationResult.Success)
            {
                await this.userManager.DeleteUserAsync(customerUser.UserName);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        // GET: CustomerUsers/Home/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var customerUser = await this.customerManagementStore.GetCustomerUserByIdAsync(id);

            if (customerUser == null)
            {
                return this.NotFound();
            }

            return this.View(customerUser);
        }

        // GET: CustomerUsers/Home/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var customerUser = await this.customerManagementStore.GetCustomerUserByIdAsync(id);

            if (customerUser == null)
            {
                return this.NotFound();
            }

            return this.View(customerUser);
        }

        // POST: CustomerUsers/Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserName,CustomerName,CreatedDate,Id")]
            CustomerUser customerUser)
        {
            if (id != customerUser.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    await this.customerManagementStore.UpdateCustomerUserAsync(customerUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await this.CustomerUserExists(customerUser.UserName))
                    {
                        return this.NotFound();
                    }

                    throw;
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(customerUser);
        }

        // GET: CustomerUsers/Home
        public async Task<IActionResult> Index()
        {
            return this.View(await this.customerManagementStore.GetCustomerUsersWithUsageAsync(
                this.User.Identity.Name.GetDomainNameFromEmail()));
        }

        private async Task<bool> CustomerUserExists(string email)
        {
            return await this.customerManagementStore.CustomerUserExistsByUserNameAsync(email);
        }
    }
}
