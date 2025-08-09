using CurrencyLoader.Extensions;
using CurrencyLoader.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName!;
        
        config
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()  
            .Build();
    })
    .ConfigureServices((hostingContext, services) =>
    {
        services.ConfigureDbContext(hostingContext.Configuration);
        services.ConfigureServices(hostingContext.Configuration);
        services.ConfigureQuartz(hostingContext.Configuration);
        services.AddLogging();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeDatabase();
}

await host.RunAsync();