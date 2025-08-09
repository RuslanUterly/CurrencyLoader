using CurrencyLoader.Infrastructure.Interfaces.Repository;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using Npgsql;

namespace CurrencyLoader.Infrastructure.Repository;

/// <summary>
/// Repository for accessing and managing currency records in a PostgreSQL database.
/// </summary>
public class CurrencyRepository : ICurrencyRepository
{
    private readonly IUnitOfWork _unitOfWork;
    
    public CurrencyRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<int?> GetIdByCodeAsync(string code, CancellationToken ct = default)
    {
        const string sql = "SELECT id FROM currencies WHERE code = @code LIMIT 1";
        
        await using NpgsqlCommand command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction);
        command.Parameters.AddWithValue("@code", code);
        
        object? result = await command.ExecuteScalarAsync(ct);
        return result is int id ? id : null;
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(string code, string name, CancellationToken ct = default)
    {
        const string sql = "INSERT INTO currencies (code, name) VALUES (@code, @name) RETURNING id";
        
        await using NpgsqlCommand cmd = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction);
        cmd.Parameters.AddWithValue("@code", code);
        cmd.Parameters.AddWithValue("@name", name);
        
        object? result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }
}