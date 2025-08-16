using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Models;
using Npgsql;

namespace CurrencyLoader.Infrastructure;

/// <summary>
/// Service for working with a database of currencies and rates.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly NpgsqlDataSource _dataSource;
    
    public DatabaseService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    /// <summary>
    /// Checks if exchange rates already exist for the given date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if data exists, otherwise false.</returns>
    public async Task<bool> IsDataExistByDateAsync(DateTime date, CancellationToken ct = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM exchange_rates WHERE date = @date LIMIT 1)";
        
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new (sql, connection);
        command.Parameters.AddWithValue("@date", date);

        object? result = await command.ExecuteScalarAsync(ct);
        return result is bool b && b;
    }

    /// <summary>
    /// Saves exchange rates for a given date. 
    /// Performs insertions inside a transaction.
    /// </summary>
    /// <param name="data">Exchange rate data (ValCurs).</param>
    /// <param name="date">Date of the rates.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task SaveExchangeRatesAsync(ValCurs data, DateTime date, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            foreach (Valute valute in data.Valutes)
            {
                int currencyId = await GetOrCreateCurrency(connection, transaction, valute, ct);
                await InsertExchangeRate(connection, transaction, currencyId, date, valute, ct);
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
        }
    }

    /// <summary>
    /// Finds existing currency by code or creates a new one if not found.
    /// </summary>
    private async Task<int> GetOrCreateCurrency(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        Valute valute,
        CancellationToken ct)
    {
        string findSql = "SELECT id FROM currencies WHERE code = @code";
        await using var findCommand = new NpgsqlCommand(findSql, connection, transaction);
        findCommand.Parameters.AddWithValue("@code", valute.CharCode);

        object? existingId = await findCommand.ExecuteScalarAsync(ct);
        
        if (existingId != null && existingId is int id)
        {
            return id;
        }
        
        string insertSql = "INSERT INTO currencies (code, name) VALUES (@code, @name) RETURNING id";
        await using NpgsqlCommand insertCommand = new NpgsqlCommand(insertSql, connection, transaction);
        insertCommand.Parameters.AddWithValue("@code", valute.CharCode);
        insertCommand.Parameters.AddWithValue("@name", valute.Name);

        object? result = await insertCommand.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    /// <summary>
    /// Inserts exchange rate into the database.
    /// If the record already exists (same currency and date), it will be ignored.
    /// </summary>
    private async Task InsertExchangeRate(
        NpgsqlConnection connection, 
        NpgsqlTransaction transaction, 
        int currencyId, 
        DateTime date, 
        Valute valute,
        CancellationToken ct)
    {
        string sql = @"
            INSERT INTO exchange_rates (currency_id, date, nominal, value, vunit_rate)
            VALUES (@currencyId, @date, @nominal, @value, @vunitRate)
            ON CONFLICT (currency_id, date) DO NOTHING";

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@currencyId", currencyId);
        command.Parameters.AddWithValue("@date", date);
        command.Parameters.AddWithValue("@nominal", valute.Nominal);
        command.Parameters.AddWithValue("@value", valute.Value);
        command.Parameters.AddWithValue("@vunitRate", valute.VunitRate);

        await command.ExecuteNonQueryAsync(ct);
    }
}