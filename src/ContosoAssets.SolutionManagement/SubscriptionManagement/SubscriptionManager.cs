using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using ContosoAssets.SolutionManagement.Data;
using ContosoAssets.Utils;
using Microsoft.EntityFrameworkCore;

namespace ContosoAssets.SolutionManagement.SubscriptionManagement
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly CustomerManagementDbContext context;
        private readonly IFulfillmentManager fulfillmentManager;

        public SubscriptionManager(CustomerManagementDbContext context, IFulfillmentManager fulfillmentManager)
        {
            this.context = context;
            this.fulfillmentManager = fulfillmentManager;
        }

        public async Task AddUsageAsync(
            string user,
            string company,
            Type callerType,
            string memberName)
        {
            var customer = await this.context.CustomerUsers.FirstOrDefaultAsync(u => u.UserName == user);

            if (customer == null)
            {
                return;
            }

            var usage = new AppUse
            {
                Id = Guid.NewGuid(),
                CustomerUserId = customer.Id,
                Company = company,
                Operation = $"{callerType.FullName}:{memberName}",
                Timestamp = DateTimeOffset.UtcNow
            };

            await this.context.Usages.AddAsync(usage);
            await this.context.SaveChangesAsync();
        }

        public async Task<SolutionManagerOperationResult> CancelSubscriptionAsync(string userName)
        {
            var customer = await this.GetCustomerAsync(userName);
            var subscription =
                await this.context.CustomerSubscriptions.FirstOrDefaultAsync(s => s.CustomerId == customer.Id);
            if (subscription == default)
            {
                return GenerateSingleError($"No subscription is found for customer {customer.CustomerName}.");
            }

            var result = await this.CheckAndRunAzureSubscriptionOperation(
                subscription.SkuId,
                async () => await this.fulfillmentManager.RequestCancelSubscriptionAsync(subscription.SubscriptionId));

            if (result.Succeeded)
            {
                subscription.State = SubscriptionState.Cancelled;
                subscription.LastOperationTime = DateTimeOffset.UtcNow;
                await this.context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IEnumerable<Sku>> GetSkusAsync(SalesChannelEnum salesChannel = SalesChannelEnum.OtherChannel)
        {
            // NOTE: added for Azure Marketplace integartion
            return await this.context.Skus.Where(s => s.SalesChannel == salesChannel).ToListAsync();
        }

        public async Task<Subscription> GetSubscriptionAsync(string userName)
        {
            var currentCustomer = await this.GetCustomerAsync(userName);

            var customerSubscriptions = this.context.CustomerSubscriptions
                .Where(s => s.CustomerId == currentCustomer.Id)
                .Include(s => s.Sku);

            // Customer must only have one subscription
            return await customerSubscriptions.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUse>> GetUsage(Expression<Func<AppUse, bool>> predicate)
        {
            return await this
                .context
                .Customers
                .Include(c => c.Users)
                .Include(c => c.Subscriptions)
                .SelectMany(c => c.Users)
                .Include(u => u.Usages)
                .SelectMany(u => u.Usages)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<bool> IsUsageInSkuLimitsAsync(string customerName)
        {
            var totalCustomerUsage = await this.context.Customers
                .Where(c => c.CustomerName == customerName)
                .Include(c => c.Users)
                .SelectMany(c => c.Users)
                .Include(u => u.Usages)
                .SelectMany(u => u.Usages)
                .ToListAsync();

            var now = DateTimeOffset.UtcNow;
            var usage = new
            {
                Total = totalCustomerUsage.Count(),
                MonthToDateUsage = totalCustomerUsage.Count(u => u.Timestamp >=
                                                                 new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0,
                                                                     TimeSpan.Zero))
            };

            var customerSku = await this.context.Customers
                .Where(c => c.CustomerName == customerName)
                .Include(c => c.Subscriptions)
                .SelectMany(c => c.Subscriptions)
                .Include(s => s.Sku)
                .Select(s => s.Sku)
                .FirstOrDefaultAsync();

            if (customerSku == default(Sku))
            {
                return false;
            }

            return customerSku.MonthlyLimit > usage.MonthToDateUsage;
        }

        public async Task<SolutionManagerOperationResult> ProvisionSubscriptionWithAdminAsync(string userName,
            CustomerRegionEnum? customerRegion,
            string fullName,
            Guid planId,
            Guid subscrptionId)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Users = new List<CustomerUser>(),
                CustomerName = userName.GetDomainNameFromEmail()
            };

            var customerUser = new CustomerUser
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                CustomerName = userName.GetDomainNameFromEmail(),
                CustomerRegion = customerRegion,
                FullName = fullName
            };

            customer.Users.Add(customerUser);

            // We are Contoso's commerce engine part, we simply create a new subscription.
            // We distinguish the Contoso or Azure Marketplace subscription with the SKU
            var subscription = new Subscription
            {
                CustomerId = customer.Id,
                Customer = customer,
                SkuId = planId,
                State = SubscriptionState.Complete,
                SubscriptionId = subscrptionId,
                SubscriptionName = userName.GetDomainNameFromEmail(),
                LastOperationTime = DateTimeOffset.UtcNow,
                CreatedTime = DateTimeOffset.UtcNow
            };

            var existingSubscription =
                await this.context.CustomerSubscriptions.FirstOrDefaultAsync(s =>
                    s.CustomerId == subscription.CustomerId);
            if (existingSubscription != default)
            {
                // Sample limitation, a customer can only have one subscription
                return SolutionManagerOperationResult.Failed(
                    new SolutionManagementError {Description = "A customer can have one subscription only."});
            }

            await this.context.CustomerSubscriptions.AddAsync(subscription);
            await this.context.SaveChangesAsync();

            return SolutionManagerOperationResult.Success;
        }

        public async Task<SolutionManagerOperationResult> ReactivateSubscriptionAsync(Subscription subscription)
        {
            var customerSubscription =
                await this.context.CustomerSubscriptions.FirstOrDefaultAsync(s =>
                    s.SubscriptionId == subscription.SubscriptionId);
            if (customerSubscription != default)
            {
                customerSubscription.State = subscription.State;
                customerSubscription.LastOperationTime = DateTimeOffset.UtcNow;
                await this.context.SaveChangesAsync();

                return SolutionManagerOperationResult.Success;
            }

            return SolutionManagerOperationResult.Failed(new SolutionManagementError
            {
                Description = "Subscription not found."
            });
        }

        public async Task<SolutionManagerOperationResult> UpdateSubscriptionWithAdminNameAsync(string administratorName,
            Guid newSkuId)
        {
            var customer = await this.GetCustomerAsync(administratorName);
            var subscription =
                await this.context.CustomerSubscriptions.FirstOrDefaultAsync(s => s.CustomerId == customer.Id);
            if (subscription == default)
            {
                return SolutionManagerOperationResult.Failed(new SolutionManagementError
                {
                    Description = $"No subscription is found for customer {customer.CustomerName}."
                });
            }

            var sku = await this.context.Skus.FirstOrDefaultAsync(s => s.Id == newSkuId);

            var result = await this.CheckAndRunAzureSubscriptionOperation(
                sku.Id,
                async () => await this.fulfillmentManager.RequestUpdateSubscriptionAsync(subscription.SubscriptionId,
                    sku.Name));

            if (!result.Succeeded)
            {
                return result;
            }

            subscription.Sku = sku;
            subscription.SkuId = newSkuId;
            subscription.LastOperationTime = DateTimeOffset.UtcNow;
            await this.context.SaveChangesAsync();

            return result;
        }

        public async Task<SolutionManagerOperationResult> UpdateSubscriptionAsync(Guid subscriptionId, string planName)
        {
            var subscription = await this.context.CustomerSubscriptions.Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
            if (subscription == default)
            {
                return SolutionManagerOperationResult.Failed(new SolutionManagementError
                {
                    Description = $"No subscription is found with subscription ID {subscriptionId}."
                });
            }

            var sku = await this.context.Skus.FirstOrDefaultAsync(s => s.Name == planName);

            subscription.Sku = sku;
            subscription.SkuId = sku.Id;
            subscription.LastOperationTime = DateTimeOffset.UtcNow;
            await this.context.SaveChangesAsync();

            return SolutionManagerOperationResult.Success;
        }

        public async Task<SolutionManagerOperationResult> SuspendSubscriptionAsync(Guid subscriptionId)
        {
            var subscription =
                await this.context.CustomerSubscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
            if (subscription == default)
            {
                return SolutionManagerOperationResult.Failed(new SolutionManagementError
                {
                    Description = $"No subscription is found with subscription ID {subscriptionId}."
                });
            }

            subscription.State = SubscriptionState.Suspended;
            subscription.LastOperationTime = DateTimeOffset.UtcNow;
            await this.context.SaveChangesAsync();

            return SolutionManagerOperationResult.Success;
        }

        public async Task<SolutionManagerOperationResult> ReactivateSubscriptionAsync(Guid subscriptionId)
        {
            var customerSubscription =
                await this.context.CustomerSubscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
            if (customerSubscription != default)
            {
                customerSubscription.State = SubscriptionState.Complete;
                customerSubscription.LastOperationTime = DateTimeOffset.UtcNow;
                await this.context.SaveChangesAsync();

                return SolutionManagerOperationResult.Success;
            }

            return SolutionManagerOperationResult.Failed(new SolutionManagementError
            {
                Description = "Subscription not found."
            });
        }

        private static SolutionManagerOperationResult GenerateSingleError(string message)
        {
            return SolutionManagerOperationResult.Failed(new SolutionManagementError {Description = message});
        }

        private async Task<SolutionManagerOperationResult> CheckAndRunAzureSubscriptionOperation(Guid skuId,
            Func<Task<FulfillmentManagerOperationResult>> operation)
        {
            var sku = await this.context.Skus.FirstOrDefaultAsync(s => s.Id == skuId);
            if (sku.SalesChannel != SalesChannelEnum.Azure)
            {
                return SolutionManagerOperationResult.Failed(new SolutionManagementError
                {
                    Description = $"Sku {sku.Id} is not an Azure SKU"
                });
            }

            var result = await operation();
            if (!result.Succeeded)
            {
                return SolutionManagerOperationResult.Failed(
                    new SolutionManagementError {Description = result.ToString()});
            }

            return SolutionManagerOperationResult.Success;
        }

        private async Task<Customer> GetCustomerAsync(string userName)
        {
            var user = await this.context.CustomerUsers
                .Where(c => c.UserName == userName || c.ExternalUserName == userName).Include(c => c.Customer)
                .FirstOrDefaultAsync();
            if (user == default)
            {
                throw new ApplicationException($"No user with username {userName}");
            }

            if (user.Customer == default)
            {
                throw new ApplicationException($"No Customer found for username {userName}");
            }

            return user.Customer;
        }
    }
}
