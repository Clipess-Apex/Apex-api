using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public class EFDbContext : DbContext
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options) { }
        public DbSet<TimeEntryType> TimeEntryTypes { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<DailyTimeEntry> DailyTimeEntries { get; set; }
        public DbSet<MonthlyTimeEntry> MonthlyTimeEntries { get; set; }
        public DbSet<MonthlyWorkingDay> MonthlyWorkingDays { get; set; }
        public DbSet<TimeEntryNotification> TimeEntryNotifications { get; set; }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
        
    }
}
