namespace CurrencyLoader.Infrastructure.Interfaces.Repository;

public interface IExchangeRateRepository
{
    Task<bool> ExistsByDateAsync(DateTime date, CancellationToken ct);
    Task AddAsync(int currencyId, DateTime date, decimal value, CancellationToken ct);
}