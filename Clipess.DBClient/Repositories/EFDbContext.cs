﻿using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Logging;

namespace Clipess.DBClient.Repositories
{
    public class EFDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public EFDbContext(DbContextOptions<EFDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveState> LeaveStates { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveNotification> LeaveNotifications { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }

        
        public DbSet<TimeEntryType> TimeEntryTypes { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<DailyTimeEntry> DailyTimeEntries { get; set; }
        public DbSet<MonthlyTimeEntry> MonthlyTimeEntries { get; set; }
        public DbSet<MonthlyWorkingDay> MonthlyWorkingDays { get; set; }
        public DbSet<TimeEntryNotification> TimeEntryNotifications { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                });
                optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Employee>().ToTable("Employees");
        }
    }
}
