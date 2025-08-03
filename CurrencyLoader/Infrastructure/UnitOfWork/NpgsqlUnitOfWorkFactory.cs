using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using Npgsql;

namespace CurrencyLoader.Infrastructure.UnitOfWork;

public class NpgsqlUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlUnitOfWorkFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;   
    }

    public async Task<IUnitOfWork> CreateAsync(CancellationToken ct = default)
    {
        NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        
        return new NpgsqlUnitOfWork(connection);
    }
}