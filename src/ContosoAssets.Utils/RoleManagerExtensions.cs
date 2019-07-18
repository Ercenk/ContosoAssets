using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ContosoAssets.Utils
{
    public static class RoleManagerExtensions
    {
        public static async Task EnsureRolesExistAsync(this RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Constants.CustomerAdminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole {Name = Constants.CustomerAdminRoleName});
            }

            if (!await roleManager.RoleExistsAsync(Constants.CustomerUserRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole {Name = Constants.CustomerUserRoleName});
            }
        }
    }
}
