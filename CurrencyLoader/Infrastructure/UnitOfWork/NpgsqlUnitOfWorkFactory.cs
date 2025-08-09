using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using Npgsql;

namespace CurrencyLoader.Infrastructure.UnitOfWork;

/// <summary>
/// Factory service for creating instances of <see cref="IUnitOfWork"/>.
/// Ensures that a properly initialized unit of work is provided, typically with an open connection.
/// </summary>
public class NpgsqlUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlUnitOfWorkFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;   
    }

    /// <summary>
    /// Creates a new unit of work by opening a new <see cref="NpgsqlConnection"/> from the configured data source.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the operation. Defaults to <c>default</c>.</param>
    /// <returns>
    /// A task that returns an <see cref="IUnitOfWork"/> implementation (<see cref="NpgsqlUnitOfWork"/>) which is
    /// backed by an open <see cref="NpgsqlConnection"/>.
    /// </returns>
    /// <remarks>
    /// The method opens a connection using <see cref="NpgsqlDataSource.OpenConnectionAsync"/> and wraps it in
    /// an <see cref="NpgsqlUnitOfWork"/>. The returned unit of work is responsible for disposing/closing the connection
    /// when it is disposed by the caller.
    ///
    /// Implementations should observe <paramref name="ct"/> so callers can cancel the connection open operation.
    /// </remarks>
    public async Task<IUnitOfWork> CreateAsync(CancellationToken ct = default)
    {
        NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        
        return new NpgsqlUnitOfWork(connection);
    }
}