
﻿using Clipess.API.Services;
using Clipess.DBClient.Contracts;
using Clipess.DBClient.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using Clipess.API.Controllers;


namespace Clipess.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env; // Define the environment variable
        private const string AllowAnyOrigins = "AllowAnyOrigin";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env; // Assign the environment variable
        }

         public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        } 

        public void ConfigureServices(IServiceCollection services)
        {
             services.AddCors(c =>
            {
                c.AddPolicy(AllowAnyOrigins, options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            }); 

            // Database configuration
            services.AddDbContext<EFDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))
            // Add repositories
            services.AddScoped<IUserRepository, EFUserRepository>();
            services.AddScoped<ILeaveRepository, EFLeaveRepository>();
            services.AddScoped<ILeaveTypeRepository, EFLeaveTypeRepository>();
            services.AddScoped<ILeaveNotificationRepository, EFLeaveNotificationRepository>();
            services.AddScoped<IEmployeeRepository, EFEmployeeRepository>();
            services.AddScoped<EmailService>();
            services.AddScoped<AuthService>();

            services.AddScoped<ITimeEntryRepository, EFTimeEntryRepository>();
            services.AddScoped<IPdfGenerationRepository, EFPdfGenerationRepository>();
            services.AddScoped<IAttendanceNotificationRepository, EFAttendanceNotificationRepository>();

            // Add controllers
            services.AddControllers();

            // Add SignalR
            services.AddSignalR();

            // cloudinary
            services.AddSingleton<IConfiguration>(provider => Configuration);


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
            });

            services.AddHangfire(configuration => configuration
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
           {
               CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
               SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
               QueuePollInterval = TimeSpan.Zero,
               UseRecommendedIsolationLevel = true,
               UsePageLocksOnDequeue = true,
               DisableGlobalLocks = true
           }));

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".res"] = "application/octet-stream";
            provider.Mappings[".pexe"] = "application/x-pnacl";
            provider.Mappings[".nmf"] = "application/octet-stream";
            provider.Mappings[".mem"] = "application/octet-stream";
            provider.Mappings[".wasm"] = "application/wasm";
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(AllowAnyOrigins);
            app.UseCors(env.IsDevelopment() ? "AllowReactFrontend" : "AllowSpecificOrigins");

            app.UseStaticFiles();
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Root"))
            });

             app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Root")),
                ContentTypeProvider = provider
            });

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Root";
            });

              app.UseHangfireDashboard();

            //RecurringJob.AddOrUpdate<PdfGenerationController>(
            //"Generate-PDF",
            //controller => controller.GenerateEmployeeAttendancePDF(),
            // "25 23 * * *");
        }
    }

    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireMinutes { get; set; }
    }
}
