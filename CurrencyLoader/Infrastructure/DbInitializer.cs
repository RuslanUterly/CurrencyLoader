using Npgsql;

namespace CurrencyLoader.Infrastructure;

/// <summary>
/// Initializing the database: creating the necessary tables and indexes if they do not already exist.
/// </summary>
public class DbInitializer
{
    private readonly NpgsqlDataSource _dataSource;
    
    public DbInitializer(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    /// <summary>
    /// Initializes the database structure:
    /// - Creates a currency table (currencies)
    /// - Creates a currency rate table (exchange_rates)
    /// - Creates a unique index on the exchange_rates table for the pair (currency_id, date)
    /// </summary>
    public async Task InitializeDatabase()
    {
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
    
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = new(createTablesSql, connection);
        await command.ExecuteNonQueryAsync();
    }
}