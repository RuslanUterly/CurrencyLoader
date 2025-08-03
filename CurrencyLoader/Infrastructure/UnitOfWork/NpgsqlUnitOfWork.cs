using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using Npgsql;

namespace CurrencyLoader.Infrastructure.UnitOfWork;

public class NpgsqlUnitOfWork : IUnitOfWork
{
    private bool _committed;

    public NpgsqlUnitOfWork(NpgsqlConnection connection)
    {
        Connection = connection;
        _committed = false;
    }
    
    public NpgsqlConnection Connection { get; }
    public NpgsqlTransaction Transaction { get; private set; }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (Transaction is not null) return;
        
        Transaction = await Connection.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (Transaction is null) return;
        
        await Transaction.CommitAsync(ct);
        _committed = true;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (! _committed && Transaction is not null)
        {
            await Transaction.RollbackAsync(ct);
        }
    }

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