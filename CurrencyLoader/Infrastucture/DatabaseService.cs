using System.Globalization;
using CurrencyLoader.Models;
using Npgsql;

namespace CurrencyLoader.Infrastucture;

public class DatabaseService
{
    private readonly string _connectionString;
    
    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task InitializeDatabase()
    {
        using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();
    
        string createTablesSql = @"
            CREATE TABLE IF NOT EXISTS currencies (
                id SERIAL PRIMARY KEY,
                code VARCHAR(10) UNIQUE NOT NULL,
                name VARCHAR(100) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS exchange_rates (
                id SERIAL PRIMARY KEY,
                currency_id INTEGER NOT NULL REFERENCES currencies(id),
                date DATE NOT NULL,
                value NUMERIC(10,4) NOT NULL
            );

            CREATE UNIQUE INDEX IF NOT EXISTS
                idx_exchange_rates_currency_date
            ON exchange_rates(currency_id, date);
        ";
    
        await using NpgsqlCommand command = new(createTablesSql, connection);
        await command.ExecuteNonQueryAsync();
    }
    
    public async Task<bool> IsDataExistForDate(DateTime date)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
    
        const string sql = "SELECT EXISTS(SELECT 1 FROM exchange_rates WHERE date = @date LIMIT 1)";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@date", date);
        
        return (bool)await command.ExecuteScalarAsync();
    }

    public async Task SaveExchangeRates(ValCurs data, DateTime date)
    {
        using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

        try
        {
            foreach (Valute valute in data.Valutes)
            {
                int currencyId = await GetOrCreateCurrency(connection, transaction, valute);
                await InsertExchangeRate(connection, transaction, currencyId, date, valute);
            }
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
        }
    }
    
    private async Task<int> GetOrCreateCurrency(
        NpgsqlConnection connection, 
        NpgsqlTransaction transaction, 
        Valute valute)
    {
        string findSql = "SELECT id FROM currencies WHERE code = @code";
        await using var findCommand = new NpgsqlCommand(findSql, connection, transaction);
        findCommand.Parameters.AddWithValue("@code", valute.CharCode);
    
        object? existingId = await findCommand.ExecuteScalarAsync();
        if (existingId != null) return (int)existingId;
    
        string insertSql = "INSERT INTO currencies (code, name) VALUES (@code, @name) RETURNING id";
        await using NpgsqlCommand insertCommand = new NpgsqlCommand(insertSql, connection, transaction);
        insertCommand.Parameters.AddWithValue("@code", valute.CharCode);
        insertCommand.Parameters.AddWithValue("@name", valute.Name);
    
        return (int)await insertCommand.ExecuteScalarAsync();
    }
    
    private async Task InsertExchangeRate(
        NpgsqlConnection connection, 
        NpgsqlTransaction transaction, 
        int currencyId, 
        DateTime date, 
        Valute valute)
    {
        decimal value = decimal.Parse(valute.Value, CultureInfo.GetCultureInfo("ru-RU"));
    
        string sql = @"
        INSERT INTO exchange_rates (currency_id, date, value)
        VALUES (@currencyId, @date, @value)
        ON CONFLICT (currency_id, date) DO NOTHING";
    
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@currencyId", currencyId);
        command.Parameters.AddWithValue("@date", date);
        command.Parameters.AddWithValue("@value", value);
    
        await command.ExecuteNonQueryAsync();
    }
}