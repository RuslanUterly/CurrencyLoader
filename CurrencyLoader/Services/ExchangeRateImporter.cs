using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Models;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyLoader.Services;

public class ExchangeRateImporter : IExchangeRateImporter
{
    private readonly ICurrencyService _currencyService;
    private readonly IExchangeRateChecker _checker;
    private readonly IExchangeRateSaver _saver;
    private readonly ILogger<ExchangeRateImporter> _logger;

    public ExchangeRateImporter(ICurrencyService currencyService, ILogger<ExchangeRateImporter> logger, IExchangeRateSaver saver, IExchangeRateChecker checker)
    {
        _currencyService = currencyService;
        _logger = logger;
        _saver = saver;
        _checker = checker;
    }

    public async Task ImportAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        try
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (await _checker.CheckAsync(date, ct))
                {
                    continue;
                }
        
                ValCurs? exchangeRates = await _currencyService.GetExchangeRatesAsync(date, ct);
                
                if (exchangeRates != null)
                {
                    await _saver.SaveAsync(exchangeRates, date, ct);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error import data: {ex}", ex.Message);
        }
    }
}