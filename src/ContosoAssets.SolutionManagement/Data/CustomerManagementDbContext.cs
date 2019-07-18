using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ContosoAssets.SolutionManagement.Data
{
    public class CustomerManagementDbContext : DbContext
    {
        public CustomerManagementDbContext(DbContextOptions<CustomerManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Subscription> CustomerSubscriptions { get; set; }
        public DbSet<CustomerUser> CustomerUsers { get; set; }

        public DbSet<Sku> Skus { get; set; }
        public DbSet<AppUse> Usages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sku>(entity =>
            {
                // Seed the SKUs as an example
                entity.HasData(
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "None",
                        Description = "None",
                        MonthlyCost = 0,
                        MonthlyLimit = int.MaxValue,
                        SalesChannel = SalesChannelEnum.All
                    },
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "Silver",
                        Description = "Silver",
                        MonthlyCost = 9.99,
                        MonthlyLimit = 2,
                        SalesChannel = SalesChannelEnum.OtherChannel
                    },
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gold",
                        Description = "Gold",
                        MonthlyCost = 19.99,
                        MonthlyLimit = 5,
                        SalesChannel = SalesChannelEnum.OtherChannel
                    },
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "Platinum",
                        Description = "Platinum",
                        MonthlyCost = 29.99,
                        MonthlyLimit = 7,
                        SalesChannel = SalesChannelEnum.OtherChannel
                    },

                    // NOTE: added for Azure Marketplace integartion
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "Silver",
                        Description = "Silver",
                        MonthlyCost = 9.99,
                        MonthlyLimit = 2,
                        SalesChannel = SalesChannelEnum.Azure
                    },
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gold",
                        Description = "Gold",
                        MonthlyCost = 19.99,
                        MonthlyLimit = 5,
                        SalesChannel = SalesChannelEnum.Azure
                    },
                    new Sku
                    {
                        Id = Guid.NewGuid(),
                        Name = "Platinum",
                        Description = "Platinum",
                        MonthlyCost = 29.99,
                        MonthlyLimit = 7,
                        SalesChannel = SalesChannelEnum.Azure
                    });

                entity.HasKey(s => s.Id);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(customer => customer.Id);
            });

            modelBuilder.Entity<CustomerUser>(entity =>
            {
                entity.HasKey(customerUser => customerUser.Id);

                entity
                    .HasOne(customerUser => customerUser.Customer)
                    .WithMany(customer => customer.Users)
                    .HasForeignKey(customerUser => customerUser.CustomerId);
            });

            modelBuilder.Entity<AppUse>(entity =>
            {
                entity.HasKey(appUse => appUse.Id);

                entity
                    .HasOne(appUse => appUse.CustomerUser)
                    .WithMany(user => user.Usages)
                    .HasForeignKey(appUse => appUse.CustomerUserId);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(subscription => subscription.SubscriptionId);

                entity
                    .HasOne(subscription => subscription.Customer)
                    .WithMany(customer => customer.Subscriptions)
                    .HasForeignKey(subscription => subscription.CustomerId);

                entity
                    .HasOne(subscription => subscription.Sku)
                    .WithMany(sku => sku.Subscriptions)
                    .HasForeignKey(subscription => subscription.SkuId);
            });
        }
    }
}
