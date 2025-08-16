using CurrencyLoader.Models;

namespace CurrencyLoader.Infrastructure.Interfaces;

/// <summary>
/// Provides database operations for exchange rate data.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Checks if exchange rate data exists for the given date.
    /// </summary>
    /// <param name="date">The date to check for existing exchange rates.</param>
    /// <param name="ct">Token for canceling the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous check operation. 
    /// The task result contains <c>true</c> if data exists, otherwise <c>false</c>.
    /// </returns>
    Task<bool> IsDataExistByDateAsync(DateTime date, CancellationToken ct);
    
    /// <summary>
    /// Saves exchange rates for the given date.
    /// </summary>
    /// <param name="data">The collection of exchange rates to save.</param>
    /// <param name="date">The date to associate with the exchange rates.</param>
    /// <param name="ct">Token for canceling the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// </returns>
    Task SaveExchangeRatesAsync(ValCurs data, DateTime date, CancellationToken ct);
}