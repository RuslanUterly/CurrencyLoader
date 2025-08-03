namespace CurrencyLoader.Infrastructure.Interfaces.Repository;

public interface ICurrencyRepository
{
    Task<int?> GetIdByCodeAsync(string code, CancellationToken ct);
    Task<int> AddAsync(string code, string name, CancellationToken ct);
}