namespace CurrencyLoader.Services.Interfaces;

/// <summary>
/// Interface for importing exchange rates for a specific date range.
/// </summary>
public interface IExchangeRateImporter
{
    /// <summary>
    /// Imports exchange rates for the given inclusive date range.
    /// </summary>
    /// <param name="startDate">The start date of the range to import.</param>
    /// <param name="endDate">The end date of the range to import.</param>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>
    /// A task that represents the asynchronous import operation.
    /// </returns>
    Task ImportAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}