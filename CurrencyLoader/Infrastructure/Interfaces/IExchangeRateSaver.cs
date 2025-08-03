using CurrencyLoader.Models;

namespace CurrencyLoader.Infrastructure.Interfaces;

public interface IExchangeRateSaver
{
    Task SaveAsync(ValCurs data, DateTime date, CancellationToken ct);
}