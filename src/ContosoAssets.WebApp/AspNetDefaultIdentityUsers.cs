using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ContosoAssets.WebApp
{
    public class AspNetDefaultIdentityUsers : IUserManagerAdapter
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AspNetDefaultIdentityUsers(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<bool> DeleteUserAsync(string userName)
        {
            var foundUser = await this.userManager.FindByEmailAsync(userName);
            if (foundUser != default(IdentityUser))
            {
                await this.userManager.DeleteAsync(foundUser);

                return true;
            }

            return false;
        }

        public async Task<bool> IsUserInRole(string userName, string roleName)
        {
            var foundUser = await this.userManager.FindByEmailAsync(userName);
            if (foundUser == default(IdentityUser))
            {
                return false;
            }

            return await this.userManager.IsInRoleAsync(foundUser, roleName);
        }
    }
}
