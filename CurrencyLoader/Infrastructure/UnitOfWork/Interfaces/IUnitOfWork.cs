using Npgsql;

namespace CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;

/// <summary>
/// Interface for the Unit of Work pattern — represents a single unit of work with the database.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Active connection to the PostgreSQL database.
    /// </summary>
    NpgsqlConnection Connection { get; }

    /// <summary>
    /// Active transaction. Must be started using BeginTransactionAsync.
    /// </summary>
    NpgsqlTransaction Transaction { get; }

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <param name="ct">Canceling an operation.</param>
    Task BeginTransactionAsync(CancellationToken ct);

    /// <summary>
    /// Commits all changes made during the current transaction.
    /// </summary>
    /// <param name="ct">Canceling an operation.</param>
    Task CommitAsync(CancellationToken ct);

    /// <summary>
    /// Rolls back all changes made during the current transaction.
    /// </summary>
    /// <param name="ct">Canceling an operation.</param>
    Task RollbackAsync(CancellationToken ct);
}
