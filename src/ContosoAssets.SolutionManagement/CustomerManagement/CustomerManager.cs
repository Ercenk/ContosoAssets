using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoAssets.SolutionManagement.CustomerManagement
{
    public class CustomerManager : ICustomerManager
    {
        private readonly CustomerManagementDbContext context;

        public CustomerManager(
            CustomerManagementDbContext context)
        {
            this.context = context;
        }

        public async Task<SolutionManagerOperationResult> AddCustomerAsync(Customer customer)
        {
            try
            {
                this.context.Customers.Add(customer);

                await this.context.SaveChangesAsync();
                return SolutionManagerOperationResult.Success;
            }
            catch (Exception ex)
            {
                return ReturnFail(ex);
            }
        }

        public async Task<SolutionManagerOperationResult> AddUserAsync(CustomerUser customerUser)
        {
            try
            {
                var customer = await this.context.Customers
                    .Where(c => c.CustomerName == customerUser.CustomerName)
                    .Include(c => c.Users)
                    .FirstAsync();

                customer.Users.Add(customerUser);

                await this.context.SaveChangesAsync();

                return SolutionManagerOperationResult.Success;
            }
            catch (Exception ex)
            {
                return ReturnFail(ex);
            }
        }

        public async Task<bool> CustomerUserExistsByUserNameAsync(string email)
        {
            return await this.context.CustomerUsers.AnyAsync(u => u.UserName == email);
        }

        public async Task<SolutionManagerOperationResult> DeleteCustomerUserAsync(CustomerUser customerUser)
        {
            var foundCustomerUser = await this.context.CustomerUsers.FirstOrDefaultAsync(
                u => u.UserName == customerUser.UserName);
            if (foundCustomerUser != default(CustomerUser))
            {
                this.context.CustomerUsers.Remove(foundCustomerUser);
                await this.context.SaveChangesAsync();

                return SolutionManagerOperationResult.Success;
            }

            return SolutionManagerOperationResult.Failed();
        }

        public Task<string> GetActivationCodeAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomerUserWithUsage> GetCustomerUserByIdAsync(Guid userId)
        {
            return await this.context.CustomerUsers
                .Where(m => m.Id == userId)
                .Include(u => u.Usages)
                .Select(cu => new CustomerUserWithUsage(cu))
                .FirstAsync();
        }

        public async Task<CustomerUser> GetCustomerUserByUserNameAsync(string userName)
        {
            return await this.context.CustomerUsers
                .FirstOrDefaultAsync(m => m.UserName == userName);
        }

        public async Task<IEnumerable<CustomerUser>> GetCustomerUsersAsync(string customerName)
        {
            return await this.context.CustomerUsers
                .Where(u => u.CustomerName == customerName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerUserWithUsage>> GetCustomerUsersWithUsageAsync(string customerName)
        {
            return await this.context.CustomerUsers
                .Where(u => u.CustomerName == customerName)
                .Include(u => u.Usages)
                .Select(cu => new CustomerUserWithUsage(cu))
                .ToListAsync();
        }

        public async Task<CustomerUserWithUsage> GetCustomerUserUsageAsync(string userName)
        {
            return await this.context.CustomerUsers
                .Where(u => u.UserName == userName)
                .Include(u => u.Usages)
                .Select(cu => new CustomerUserWithUsage(cu))
                .FirstAsync();
        }

        public async Task UpdateCustomerUserAsync(CustomerUser customerUser)
        {
            this.context.Update(customerUser);
            await this.context.SaveChangesAsync();
        }

        public async Task<SolutionManagerOperationResult> DeleteCustomerUserAsync(
            CustomerUser customerUser,
            CancellationToken cancellationToken)
        {
            this.context.CustomerUsers.Remove(customerUser);
            await this.context.SaveChangesAsync(cancellationToken);
            return SolutionManagerOperationResult.Success;
        }

        private static SolutionManagerOperationResult ReturnFail(Exception ex)
        {
            return SolutionManagerOperationResult.Failed(new SolutionManagementError {Description = ex.ToString()});
        }
    }
}
