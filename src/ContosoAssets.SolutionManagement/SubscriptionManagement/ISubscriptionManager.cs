using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.Data;

namespace ContosoAssets.SolutionManagement.SubscriptionManagement
{
    public interface ISubscriptionManager
    {
        Task AddUsageAsync(string user, string company, Type callerType, string memberName);

        Task<SolutionManagerOperationResult> CancelSubscriptionAsync(string userName);

        Task<IEnumerable<Sku>> GetSkusAsync(SalesChannelEnum salesChannel = SalesChannelEnum.OtherChannel);

        Task<Subscription> GetSubscriptionAsync(string userName);

        Task<IEnumerable<AppUse>> GetUsage(Expression<Func<AppUse, bool>> predicate);

        Task<bool> IsUsageInSkuLimitsAsync(string customerName);

        Task<SolutionManagerOperationResult> ProvisionSubscriptionWithAdminAsync(string adminUser,
            CustomerRegionEnum? customerRegion,
            string fullName,
            Guid planId,
            Guid subscrptionId);

        Task<SolutionManagerOperationResult> ReactivateSubscriptionAsync(Subscription subscription);

        Task<SolutionManagerOperationResult> ReactivateSubscriptionAsync(Guid subscriptionId);

        Task<SolutionManagerOperationResult> UpdateSubscriptionWithAdminNameAsync(string adminName, Guid plainId);

        Task<SolutionManagerOperationResult> UpdateSubscriptionAsync(Guid subscriptionId, string planName);

        Task<SolutionManagerOperationResult> SuspendSubscriptionAsync(Guid subscriptionId);
    }
}
