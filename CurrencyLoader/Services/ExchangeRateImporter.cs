using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Models;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyLoader.Services;

/// <summary>
/// Imports exchange rates for a range of dates by checking whether rates already exist,
/// fetching rates from a currency provider, and saving them to the storage if present.
/// </summary>
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
    
    /// <summary>
    /// Imports exchange rates for every date in the inclusive range from <paramref name="startDate"/> to <paramref name="endDate"/>.
    /// </summary>
    /// <param name="startDate">The inclusive start date of the import range.</param>
    /// <param name="endDate">The inclusive end date of the import range.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the import operation. Defaults to <c>default</c>.</param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous import operation.
    /// </returns>
    /// <remarks>
    /// For each date in the range the method:
    /// 1. Uses <see cref="_checker"/> to determine whether rates for the date already exist and skips the date when they do.
    /// 2. Calls <see cref="_currencyService"/> to retrieve <see cref="ValCurs"/> for the date.
    /// 3. If rates are returned, calls <see cref="_saver"/> to persist them.
    /// </remarks>
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