using CurrencyLoader.Extensions;
using CurrencyLoader.Infrastructure;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

IConfiguration configuration = services.AddConfiguration();

services.ConfigureDbContext(configuration);
services.ConfigureServices(configuration);
services.AddLogging();

await using ServiceProvider provider = services.BuildServiceProvider();
    
var importer = provider.GetRequiredService<IExchangeRateImporter>();
var dbInitializer = provider.GetRequiredService<DbInitializer>();
    
await dbInitializer.InitializeDatabase();
    
DateTime endDate = DateTime.Today;
DateTime startDate = endDate.AddMonths(-1);

await importer.ImportAsync(startDate, endDate, CancellationToken.None);