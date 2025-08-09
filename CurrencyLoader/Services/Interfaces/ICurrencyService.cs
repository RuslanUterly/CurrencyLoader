using CurrencyLoader.Models;

namespace CurrencyLoader.Services.Interfaces;

/// <summary>
/// Interface for retrieving exchange rates (ValCurs) for a specific date.
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Gets exchange rates for the specified date.
    /// </summary>
    /// <param name="date">The date for which to retrieve exchange rates.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that returns a <see cref="ValCurs"/> instance containing exchange rates for the given date,
    /// or <c>null</c> if rates are not available for that date.
    /// </returns>
    Task<ValCurs?> GetExchangeRatesAsync(DateTime date, CancellationToken ct);
}