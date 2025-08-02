using CurrencyLoader;
using CurrencyLoader.Infrastucture;
using CurrencyLoader.Models;
using CurrencyLoader.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
Startup.ConfigureServices(services);
await using ServiceProvider provider = services.BuildServiceProvider();
    
var currencyService = provider.GetRequiredService<CurrencyService>();
var databaseService = provider.GetRequiredService<DatabaseService>();
    
await databaseService.InitializeDatabase();
    
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