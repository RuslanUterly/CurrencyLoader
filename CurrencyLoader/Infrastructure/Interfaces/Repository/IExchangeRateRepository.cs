namespace CurrencyLoader.Infrastructure.Interfaces.Repository;

/// <summary>
/// Interface for accessing and managing exchange rate records in the data store.
/// </summary>
public interface IExchangeRateRepository
{
    /// <summary>
    /// Checks if exchange rates for the given date already exist in the data store.
    /// </summary>
    /// <param name="date">The date for which to check the existence of exchange rates.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that returns <c>true</c> if exchange rates exist for the specified date;
    /// otherwise <c>false</c>.
    /// </returns>
    Task<bool> ExistsByDateAsync(DateTime date, CancellationToken ct);
    
    /// <summary>
    /// Adds a new exchange rate record to the data store.
    /// </summary>
    /// <param name="currencyId">The ID of the associated currency.</param>
    /// <param name="date">The date of the exchange rate.</param>
    /// <param name="nominal">The nominal value for the currency.</param>
    /// <param name="value">The exchange rate value.</param>
    /// <param name="vunitRate">The exchange rate per unit of the currency.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that represents the asynchronous add operation.
    /// </returns>
    Task AddAsync(int currencyId, DateTime date,int nominal, decimal value, decimal vunitRate, CancellationToken ct);
}