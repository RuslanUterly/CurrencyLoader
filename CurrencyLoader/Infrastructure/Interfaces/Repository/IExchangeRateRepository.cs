namespace CurrencyLoader.Infrastructure.Interfaces.Repository;

public interface IExchangeRateRepository
{
    Task<bool> ExistsByDateAsync(DateTime date, CancellationToken ct);
    Task AddAsync(int currencyId, DateTime date,int nominal, decimal value, decimal vunitRate, CancellationToken ct);
}