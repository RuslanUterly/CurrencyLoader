namespace CurrencyLoader.Services.Interfaces;

public interface IExchangeRateImporter
{
    Task ImportAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}