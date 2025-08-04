using CurrencyLoader.Infrastructure.Interfaces.Repository;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using Npgsql;

namespace CurrencyLoader.Infrastructure.Repository;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public ExchangeRateRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExistsByDateAsync(DateTime date, CancellationToken ct = default)
    {
        const string sql = "SELECT EXISTS (SELECT 1 FROM exchange_rates WHERE date = @date)";
        
        await using NpgsqlCommand command = new NpgsqlCommand(sql, _unitOfWork.Connection);
        command.Parameters.AddWithValue("@date", date);
        
        object? result = await command.ExecuteScalarAsync(ct);
        return result is bool ok && ok;
    }

    public async Task AddAsync(int currencyId, DateTime date, decimal value, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO exchange_rates (currency_id, date, value)
            VALUES (@currencyId, @date, @value)
            ON CONFLICT (currency_id, date) DO NOTHING";
        
        await using NpgsqlCommand command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction);
        command.Parameters.AddWithValue("@currencyId", currencyId);
        command.Parameters.AddWithValue("@date", date);
        command.Parameters.AddWithValue("@value", value);
        
        await command.ExecuteNonQueryAsync(ct);
    }
}