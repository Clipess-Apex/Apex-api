/*using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public class EFDbContext : DbContext
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Request> Requests { get; set; }

        public DbSet<Inventory_Type> Inventory_Types_{ get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>()
                .Property(e => e.InventoryId)
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Request>()
                .Property(e => e.EmployeeId)
                .ValueGeneratedOnAddOrUpdate();
        }

    }
}
*/
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Clipess.DBClient.Repositories
{
    public class EFDbContext : DbContext
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options) { }

        
        public DbSet<Request> Requests { get; set; }
        public DbSet<InventoryType> InventoryTypes { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Employee> Employees { get; set;}
       // public DbSet<InventoryReport> InventoryReports { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}
    }
}
