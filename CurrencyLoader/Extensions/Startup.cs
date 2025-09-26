using System.Text;
using CurrencyLoader.Infrastructure;
using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Models.Options;
using CurrencyLoader.Services;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Quartz;

namespace CurrencyLoader.Extensions;

public static class Startup
{
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<NpgsqlDataSource>(sp =>
        {
            var connectionString = configuration.GetConnectionString("CurrencyDb");
            
            return new NpgsqlDataSourceBuilder(connectionString)
                .UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                .EnableParameterLogging()
                .Build();
        });
        
        services.AddTransient<IDatabaseService, DatabaseService>();
        
        services.AddScoped<DbInitializer>();
    }
    
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CbrOptions>(
            configuration.GetSection("CbrOptions"));
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        services.AddHttpClient<CurrencyService>();
        
        services.AddScoped<IExchangeRateImporter, ExchangeRateImporter>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ExchangeImportJob>();
    }

    public static void ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("ExchangeImportJob");
            q.AddJob<ExchangeImportJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ExchangeImportTrigger")
                .WithCronSchedule(configuration["Import:Cron"]));
        });
        
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}