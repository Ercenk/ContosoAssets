using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ContosoAssets.WebApp.Models
{
    public class AssetManagementDbContext : DbContext
    {
        public AssetManagementDbContext(DbContextOptions<AssetManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
    }
}
