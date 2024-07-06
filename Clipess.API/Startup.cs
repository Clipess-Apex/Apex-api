
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
using Clipess.DBClient.Infrastructure;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;//for clou
using Clipess.API.Properties.Services;


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
            _env = env; 
        }

        public void ConfigureServices(IServiceCollection services)
        {

             services.AddCors(c =>
            {
                c.AddPolicy("AllowAnyOrigins", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
                 c.AddPolicy(AllowAnyOrigins, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
                c.AddPolicy("AllowSpecificOrigin",
                    builder =>  builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    );

                    if (_env.IsDevelopment())
                {
                    c.AddPolicy("AllowReactFrontend", builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                }
                else
                {
                    c.AddPolicy("AllowSpecificOrigins", builder =>
                    {
                        builder.AllowAnyOrigin() 
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                }
            }); 
           
   

          
         
            // Database configuration
            services.AddDbContext<EFDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))
            // Add repositories
            services.AddScoped<IUserRepository, EFUserRepository>();
            services.AddScoped<ILeaveRepository, EFLeaveRepository>();
            services.AddScoped<ILeaveTypeRepository, EFLeaveTypeRepository>();
            services.AddScoped<ILeaveNotificationRepository, EFLeaveNotificationRepository>();
            services.AddScoped<EmailService>();
            services.AddScoped<AuthService>();

            services.AddScoped<ITimeEntryRepository, EFTimeEntryRepository>();
            services.AddScoped<IPdfGenerationRepository, EFPdfGenerationRepository>();
            services.AddScoped<IAttendanceNotificationRepository, EFAttendanceNotificationRepository>();

            services.AddScoped<INotificationRepository, EFNotificationRepository>();
           
            services.AddScoped<IProjectRepository, EFProjectRepository>();       
            services.AddScoped<ITaskRepository, EFTaskRepository>();
            services.AddScoped<ITeamRepository, EFTeamRepository>();
            services.AddScoped<IClientRepository, EFClientRepository>();
            services.AddScoped<ITaskPdfGenerationRepository,EFTaskPdfGenerationRepository>();

            services.AddScoped<IInventoryTypeRepository, EFInventoryTypeRepository>();
            services.AddScoped<IInventoryRepository, EFInventoryRepository>();
            services.AddScoped<IEmployeeInventoryRepository, EFEmployeeInventoryRepository>();
            services.AddScoped<IRequestRepository, EFRequestRepository>();
            services.AddScoped<IInventoryReportRepository, EFInventoryReportRepository>();

            services.AddScoped<IEmployeeRepository, EFEmployeeRepository>();            
            services.AddScoped<IEmployeeTypeRepository, EFEmployeeTypeRepository>();
            services.AddScoped<IMaritalStatusRepository, EFMaritalStatusRepository>();  
            services.AddScoped<IDepartmentRepository, EFDepartmentRepository>();
            services.AddScoped<IRoleRepository, EFRoleRepository>();

            // Add controllers
            services.AddControllers();

            // Add SignalR
            services.AddSignalR();

            services.AddSingleton(provider => Configuration);
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

            // JWT Authentication configuration
            var jwtSettingsSection = Configuration.GetSection("Jwt");
            services.Configure<JwtSettings>(jwtSettingsSection);

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
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
            app.UseCors("AllowAllOrigins");
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
            
            
            
            app.UseDefaultFiles();
            
            

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

            app.UseHttpsRedirection();
            app.UseCors("AllowAnyOrigins");
            app.UseCors(env.IsDevelopment() ? "AllowReactFrontend" : "AllowSpecificOrigins");

            app.UseStaticFiles();
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Root"))
            });

            app.UseCors("AllowSpecificOrigin");
            app.UseCors("CorsPolicy");
            
            
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
                endpoints.MapHub<SignalServer>("/signalServer");
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
