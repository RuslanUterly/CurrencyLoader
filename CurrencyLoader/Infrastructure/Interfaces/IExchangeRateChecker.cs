namespace CurrencyLoader.Infrastructure.Interfaces;

/// <summary>
/// Interface for checking whether exchange rates for a specific date already exist in the database.
/// </summary>
public interface IExchangeRateChecker
{
    /// <summary>
    /// Checks if exchange rates for the given date are already stored.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that returns <c>true</c> if the exchange rates for the specified date exist,
    /// otherwise <c>false</c>.
    /// </returns>
    Task<bool> CheckAsync(DateTime date, CancellationToken ct);
}