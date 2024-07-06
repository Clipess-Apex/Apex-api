using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public class EFDbContext : DbContext
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<MaritalStatus> MaritalStatus { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the EmployeeID to be auto-incremented if not already done
            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeID)
                .ValueGeneratedOnAdd();
        }
    }
}