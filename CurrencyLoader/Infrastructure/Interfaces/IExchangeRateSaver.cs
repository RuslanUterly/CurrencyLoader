using CurrencyLoader.Models;

namespace CurrencyLoader.Infrastructure.Interfaces;

/// <summary>
/// Interface for saving exchange rate data into the database.
/// </summary>
public interface IExchangeRateSaver
{
    /// <summary>
    /// Saves exchange rate and currency data for a specific date.
    /// </summary>
    /// <param name="data">The exchange rate data (ValCurs) to save.</param>
    /// <param name="date">The date the exchange rates correspond to.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync(ValCurs data, DateTime date, CancellationToken ct);
}