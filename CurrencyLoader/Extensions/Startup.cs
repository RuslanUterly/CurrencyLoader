using System.Text;
using CurrencyLoader.Infrastructure;
using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Infrastructure.UnitOfWork;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using CurrencyLoader.Models.Options;
using CurrencyLoader.Services;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

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
        
        services.AddSingleton<IUnitOfWorkFactory, NpgsqlUnitOfWorkFactory>();
        services.AddTransient<IExchangeRateSaver, ExchangeRateSaver>();
        services.AddTransient<IExchangeRateChecker, ExchangeRateChecker>();
        
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
    }
    
    public static IConfiguration AddConfiguration(this IServiceCollection services)
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName!;
        
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()  
            .Build();
    
        services.AddSingleton(configuration);
        
        return configuration;
    }
}