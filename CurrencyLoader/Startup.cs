using System.Text;
using CurrencyLoader.Infrastucture;
using CurrencyLoader.Models.Options;
using CurrencyLoader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyLoader;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName!;
        
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Startup>()
            .Build();
    
        services.AddSingleton(configuration);

        services.AddLogging();
        
        services.Configure<CbrOptions>(
            configuration.GetSection("CbrOptions"));
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        services.AddHttpClient<CurrencyService>();
        
        services.AddScoped<CurrencyService>();
        services.AddScoped<DatabaseService>(opt => new DatabaseService(configuration.GetValue<string>("DatabaseSettings:ConnectionString")));
    }
}