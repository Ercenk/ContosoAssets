using ContosoAssets.SolutionManagement.CustomerManagement;
using ContosoAssets.SolutionManagement.Data;
using ContosoAssets.SolutionManagement.Provisioning;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContosoAssets.SolutionManagement
{
    public static class SolutionManagementExtensions
    {
        public static void AddSolutionManagement(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CustomerManagementDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.TryAddScoped<ISubscriptionManager, SubscriptionManager>();
            services.TryAddScoped<IProvisioningManager, ProvisioningManager>();
            services.TryAddScoped<ICustomerManager, CustomerManager>();
        }
    }
}
