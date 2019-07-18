using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.Data;

namespace ContosoAssets.SolutionManagement.CustomerManagement
{
    public interface ICustomerManager
    {
        Task<SolutionManagerOperationResult> AddCustomerAsync(Customer customerUser);

        Task<SolutionManagerOperationResult> AddUserAsync(CustomerUser customerUser);

        Task<bool> CustomerUserExistsByUserNameAsync(string userUserName);

        Task<SolutionManagerOperationResult> DeleteCustomerUserAsync(CustomerUser customerUser);

        Task<string> GetActivationCodeAsync(string email);

        Task<CustomerUserWithUsage> GetCustomerUserByIdAsync(Guid userId);

        Task<CustomerUser> GetCustomerUserByUserNameAsync(string userName);

        Task<IEnumerable<CustomerUser>> GetCustomerUsersAsync(string customerName);

        Task<IEnumerable<CustomerUserWithUsage>> GetCustomerUsersWithUsageAsync(string getDomainNameFromEmail);

        Task<CustomerUserWithUsage> GetCustomerUserUsageAsync(string userName);

        Task UpdateCustomerUserAsync(CustomerUser customerUser);
    }
}
