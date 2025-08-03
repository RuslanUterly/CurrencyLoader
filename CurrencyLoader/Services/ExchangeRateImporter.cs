using CurrencyLoader.Infrastucture;
using CurrencyLoader.Models;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyLoader.Services;

public class ExchangeRateImporter : IExchangeRateImporter
{
    private readonly ICurrencyService _currencyService;
    private readonly DatabaseService _dbService;
    private readonly ILogger<ExchangeRateImporter> _logger;

    public ExchangeRateImporter(ICurrencyService currencyService, DatabaseService dbService, ILogger<ExchangeRateImporter> logger)
    {
        _currencyService = currencyService;
        _dbService = dbService;
        _logger = logger;
    }

    public async Task ImportAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (await _dbService.IsDataExistForDate(date)) continue;
        
                ValCurs? exchangeRates = await _currencyService.GetExchangeRatesAsync(date);
                if (exchangeRates != null)
                {
                    await _dbService.SaveExchangeRates(exchangeRates, date);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error import data: {ex}", ex.Message);
        }
    }
}