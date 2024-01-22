using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Services;
using Microsoft.EntityFrameworkCore;

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
            services.AddControllers();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IPersonService, PersonService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
