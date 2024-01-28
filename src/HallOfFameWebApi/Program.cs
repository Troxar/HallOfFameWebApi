using HallOfFameWebApi.Configuration;
using Serilog;

namespace HallOfFameWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(ConfigureSerilog)
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<Startup>();
                });

        public static void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var loggingOptions = context.Configuration
                .GetSection("FileLogging").Get<FileLoggingOptions>();

            if (loggingOptions?.Enable == true)
            {
                config.WriteTo.File(
                    path: loggingOptions.Path,
                    rollingInterval: loggingOptions.RollingInterval,
                    outputTemplate: loggingOptions.OutputTemplate,
                    fileSizeLimitBytes: loggingOptions.FileSizeLimitBytes,
                    retainedFileCountLimit: loggingOptions.RetainedFileCountLimit
                );
            }
        }
    }
}
