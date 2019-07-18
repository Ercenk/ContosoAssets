using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ContosoAssets.Utils
{
    public static class IdentityExtensions
    {
        public static bool IsUserInRole(this UserManager<IdentityUser> userManager, ClaimsPrincipal pageUser,
            string role)
        {
            if (pageUser.Identity.Name == null)
            {
                return false;
            }

            var user = userManager.FindByEmailAsync(pageUser.Identity.Name).Result;
            return user != null && userManager.GetRolesAsync(user).Result.Any(r => r == role);
        }
    }
}
