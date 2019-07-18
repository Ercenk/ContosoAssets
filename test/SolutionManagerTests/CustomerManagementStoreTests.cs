using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using ContosoAssets.SolutionManagement.Data;
using ContosoAssets.SolutionManagement.UserManagement;
using Xunit;

namespace SolutionManagerTests
{
    public class CustomerManagementStoreTests
    {
        private DbContextOptionsBuilder<CustomerManagementDbContext> builder;
        private CustomerManager customerManagementStore;
        private CustomerManagementDbContext dbContext;

        public CustomerManagementStoreTests()
        {
            var connectionStringBuilder =
                new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            this.builder = new DbContextOptionsBuilder<CustomerManagementDbContext>();
            this.builder.UseSqlite(connection);

            this.dbContext = new CustomerManagementDbContext(this.builder.Options);

            this.customerManagementStore = new CustomerManager(this.dbContext);
        }

        [Fact]
        public async Task CanAddCustomer()
        {
            var sku = await this.dbContext.Skus.FirstAsync();
            var result = await this.customerManagementStore.AddCustomerAsync(
                new Customer
                {
                    CustomerName = "Customer",
                    Id = Guid.NewGuid()
                });

            var context = new CustomerManagementDbContext(this.builder.Options);

            var customer = await context.Customers.FirstAsync();

            Assert.True(result.Succeeded);
            Assert.Equal("Customer", customer.CustomerName);
        }

        [Fact]
        public async Task CanAddCustomerUser()
        {
            var sku = await this.dbContext.Skus.FirstAsync();
            var customerId = Guid.NewGuid();

            var result = await this.customerManagementStore.AddCustomerAsync(
                new Customer
                {
                    CustomerName = "Customer",
                    Id = customerId
                });

            var context = new CustomerManagementDbContext(this.builder.Options);
            var customer = await context.Customers.FirstAsync();

            result = await this.customerManagementStore.AddUserAsync(
                new CustomerUser
                {
                    UserName = "user",
                    CustomerName = customer.CustomerName,
                    Id = Guid.NewGuid(),
                    CustomerId = customerId
                });

            Assert.True(result.Succeeded);

            var customerUser = await context.CustomerUsers.FirstAsync();

            Assert.Equal("user", customerUser.UserName);
        }
    }
}
