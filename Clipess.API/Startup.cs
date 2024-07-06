

using Clipess.DBClient.Contracts;
using Clipess.DBClient.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using CloudinaryDotNet;//for clou
using Microsoft.Extensions.Configuration;// for clou




namespace Clipess.API
{
    public class Startup
    {
        private const string AllowAnyOrigins = "AllowAnyOrigin";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAnyOrigins, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddDbContext<EFDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IConfiguration>(provider => Configuration);

            // Configure DBClient dependency injection repositories
            
            services.AddScoped<IInventoryTypeRepository, EFInventoryTypeRepository>();
            services.AddScoped<IInventoryRepository, EFInventoryRepository>();
            services.AddScoped<IEmployeeInventoryRepository, EFEmployeeInventoryRepository>();
            services.AddScoped<IRequestRepository, EFRequestRepository>();
            services.AddScoped<INotificationRepository, EFNotificationRepository>();
            services.AddScoped<IInventoryReportRepository, EFInventoryReportRepository>();





            services.AddSignalR();

            services.AddControllers();

            // Add logging
            services.AddLogging(builder =>
            {
                // Configure Log4Net
                builder.AddLog4Net();
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "Root";
            });//'Root'
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(AllowAnyOrigins);

            // Serve the static files for the React frontend
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // Configure default file mapping for the React SPA
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new[] { "index.html" }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // Configure SPA
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Root";


            });
        }
    }
}


