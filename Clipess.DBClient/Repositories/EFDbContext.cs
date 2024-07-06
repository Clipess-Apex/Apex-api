using Clipess.DBClient.EntityModels;
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

       
        public DbSet<Employee> Employees { get; set;}
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<FormUser> Users { get; set; }
   

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

    }
}