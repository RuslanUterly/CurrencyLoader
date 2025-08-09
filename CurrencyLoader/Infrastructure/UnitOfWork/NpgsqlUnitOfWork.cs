using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using Npgsql;

namespace CurrencyLoader.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of work implementation that wraps an open <see cref="NpgsqlConnection"/> and an optional
/// <see cref="NpgsqlTransaction"/> for transactional operations.
/// </summary>
/// <remarks>
/// The unit of work supports beginning a transaction, committing, rolling back and disposing the
/// underlying connection and transaction. If a transaction is not committed when the unit of work
/// is disposed, it will be rolled back automatically.
/// </remarks>
public class NpgsqlUnitOfWork : IUnitOfWork
{
    private bool _committed;

    public NpgsqlUnitOfWork(NpgsqlConnection connection)
    {
        Connection = connection;
        _committed = false;
    }
    
    /// <inheritdoc/>
    public NpgsqlConnection Connection { get; }
    
    /// <inheritdoc/>
    public NpgsqlTransaction Transaction { get; private set; }

    /// <summary>
    /// Begins a database transaction if one has not already been started.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is idempotent: calling it when a transaction already exists will have no effect.
    /// </remarks>
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (Transaction is not null) return;
        
        Transaction = await Connection.BeginTransactionAsync(ct);
    }

    /// <summary>
    /// Commits the current transaction if one exists.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// After a successful commit the unit of work marks the transaction as committed so that subsequent
    /// disposal will not attempt to rollback the transaction.
    /// </remarks>
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (Transaction is null) return;
        
        await Transaction.CommitAsync(ct);
        _committed = true;
    }

    /// <summary>
    /// Rolls back the current transaction if it exists and has not been committed.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is safe to call multiple times. If the transaction is already committed or does not exist,
    /// the method returns without action.
    /// </remarks>
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (! _committed && Transaction is not null)
        {
            await Transaction.RollbackAsync(ct);
        }
    }

    /// <summary>
    /// Asynchronously disposes the unit of work, rolling back any uncommitted transaction and
    /// disposing the transaction and connection.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
    /// <remarks>
    /// If a transaction exists and has not been committed, the dispose will attempt to rollback the transaction.
    /// The connection is disposed afterwards.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        if (! _committed && Transaction is not null)
        {
            await Transaction.RollbackAsync();
            await Transaction.DisposeAsync();
        }
        
        await Connection.DisposeAsync();
    }
}