using System.Security.Claims;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using Microsoft.AspNetCore.Identity;

namespace ContosoAssets.SolutionManagement
{
    public static class UserManagerExtensions
    {
        public static bool IsUserSubscriptionActive(
            this UserManager<IdentityUser> userManager,
            ClaimsPrincipal pageUser,
            ISubscriptionManager subscriptionManager)
        {
            if (pageUser.Identity.Name == null)
            {
                return false;
            }

            var subscription = subscriptionManager.GetSubscriptionAsync(pageUser.Identity.Name).Result;

            return subscription.State == SubscriptionState.Complete;
        }
    }
}
