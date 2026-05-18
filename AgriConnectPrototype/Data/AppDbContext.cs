using AgriConnectPrototype.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriConnectPrototype.Data
{
    // Database context for the Farmers Management System
    // Connects the application models to SQL Server

    public class AppDbContext : DbContext
    {
        // Configure database connection settings
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Farmer table
        public DbSet<Farmer> Farmers { get; set; }

        // Supply contract table
        public DbSet<SupplyContract> SupplyContracts { get; set; }

        // Farm service request table
        public DbSet<FarmServiceRequest> FarmServiceRequests { get; set; }
    }
}