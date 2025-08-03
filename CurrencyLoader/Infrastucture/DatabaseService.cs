using System.Globalization;
using CurrencyLoader.Models;
using Npgsql;

namespace CurrencyLoader.Infrastucture;

public class DatabaseService
{
    private readonly NpgsqlDataSource _dataSource;
    
    public DatabaseService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    public async Task<bool> IsDataExistForDate(DateTime date)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM exchange_rates WHERE date = @date LIMIT 1)";
        
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = new (sql, connection);
        command.Parameters.AddWithValue("@date", date);
        
        object? result = await command.ExecuteScalarAsync();
        return result is bool b && b;
    }

    public async Task SaveExchangeRates(ValCurs data, DateTime date)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
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
        
        if (existingId != null && existingId is int id)
        {
            return id;
        }
    
        string insertSql = "INSERT INTO currencies (code, name) VALUES (@code, @name) RETURNING id";
        await using NpgsqlCommand insertCommand = new NpgsqlCommand(insertSql, connection, transaction);
        insertCommand.Parameters.AddWithValue("@code", valute.CharCode);
        insertCommand.Parameters.AddWithValue("@name", valute.Name);
    
        object? result = await insertCommand.ExecuteScalarAsync();
        return Convert.ToInt32(result);
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