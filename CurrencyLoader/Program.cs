using CurrencyLoader;
using CurrencyLoader.Extensions;
using CurrencyLoader.Infrastucture;
using CurrencyLoader.Models;
using CurrencyLoader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

IConfiguration configuration = services.AddConfiguration();

services.ConfigureDbContext(configuration);
services.ConfigureServices(configuration);
services.AddLogging();

await using ServiceProvider provider = services.BuildServiceProvider();
    
var currencyService = provider.GetRequiredService<CurrencyService>();
var databaseService = provider.GetRequiredService<DatabaseService>();
var dbInitializer = provider.GetRequiredService<DbInitializer>();
    
await dbInitializer.InitializeDatabase();
    
DateTime endDate = DateTime.Today;
DateTime startDate = endDate.AddMonths(-1);
    
for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
{
    if (await databaseService.IsDataExistForDate(date)) continue;
        
    ValCurs? exchangeRates = await currencyService.GetExchangeRates(date);
    if (exchangeRates != null)
    {
        await databaseService.SaveExchangeRates(exchangeRates, date);
    }
}