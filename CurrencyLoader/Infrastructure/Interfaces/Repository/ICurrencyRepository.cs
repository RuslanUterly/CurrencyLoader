namespace CurrencyLoader.Infrastructure.Interfaces.Repository;

/// <summary>
/// Interface for accessing and managing currency records in the data store.
/// </summary>
public interface ICurrencyRepository
{
    /// <summary>
    /// Gets the identifier of a currency by its code.
    /// </summary>
    /// <param name="code">The currency code (e.g., "USD", "EUR").</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that returns the currency ID if found; otherwise <c>null</c>.
    /// </returns>
    Task<int?> GetIdByCodeAsync(string code, CancellationToken ct);
    
    /// <summary>
    /// Adds a new currency record to the data store.
    /// </summary>
    /// <param name="code">The currency code (e.g., "USD", "EUR").</param>
    /// <param name="name">The name of the currency.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that returns the ID of the newly created currency.
    /// </returns>
    Task<int> AddAsync(string code, string name, CancellationToken ct);
}