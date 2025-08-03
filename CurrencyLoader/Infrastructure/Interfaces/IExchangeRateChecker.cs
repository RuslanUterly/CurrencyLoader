namespace CurrencyLoader.Infrastructure.Interfaces;

public interface IExchangeRateChecker
{
    Task<bool> CheckAsync(DateTime date, CancellationToken ct);
}