using Asp.Versioning;
using HallOfFameWebApi.Configuration;
using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace HallOfFameWebApi
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var swaggerOptions = new SwaggerOptions();
            _config.GetSection("Swagger").Bind(swaggerOptions);

            services.Configure<SwaggerOptions>(options =>
            {
                options.JsonRoute = swaggerOptions.JsonRoute;
                options.Versions = swaggerOptions.Versions;
            });

            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                var assemblyName = typeof(Startup).Assembly.GetName().Name;
                foreach (string version in swaggerOptions.Versions)
                {
                    var versionName = $"v{version}";
                    options.SwaggerDoc(versionName, new OpenApiInfo { Title = assemblyName, Version = versionName });
                }
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_config.GetConnectionString("DefaultConnection"));
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IPersonService, PersonService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var swaggerOptions = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;
                    foreach (string version in swaggerOptions.Versions)
                    {
                        string versionName = $"v{version}";
                        string url = $"/{swaggerOptions.JsonRoute.Replace("{documentName}", versionName)}";
                        options.SwaggerEndpoint(url, $"api {versionName}");
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
