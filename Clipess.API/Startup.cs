
using Clipess.DBClient.Contracts;
using Clipess.DBClient.Repositories;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.FileProviders;

using Clipess.DBClient.Infrastructure;


namespace Clipess.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env; 
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
            services.AddDbContext<EFDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))); 

            //Cloudinary
            services.AddSingleton(provider => Configuration);

            // Configure DBClient dependency injection repositories
           
            services.AddScoped<INotificationRepository, EFNotificationRepository>();
           
            services.AddScoped<IProjectRepository, EFProjectRepository>();       
            services.AddScoped<ITaskRepository, EFTaskRepository>();
            services.AddScoped<ITeamRepository, EFTeamRepository>();
            services.AddScoped<IClientRepository, EFClientRepository>();
            services.AddScoped<ITaskPdfGenerationRepository,EFTaskPdfGenerationRepository>();
           
            services.AddSignalR();

            // Configure DBClient dependency injection repositories
            services.AddControllers();

            // cloudinary
            services.AddSingleton<IConfiguration>(provider => Configuration);

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddLog4Net();
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "Root";
            });

         

           
        }

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

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAnyOrigins");

            // Serve the static files for the React frontend
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // Configure default file mapping for the React SPA
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new[] { "index.html" }
            });
            
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Root"))
            });

            app.UseCors("AllowSpecificOrigin");
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalServer>("/signalServer");
            });


            // Configure SPA
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Root";
            });

            
        }
    }

   
}
