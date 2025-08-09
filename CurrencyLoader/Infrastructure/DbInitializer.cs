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
                nominal INT NOT NULL,
                value NUMERIC(10,4) NOT NULL,
                vunit_rate DECIMAL(20,10) NOT NULL
            );

            -- Add new columns for exchange_rates
            ALTER TABLE IF EXISTS exchange_rates
                ADD COLUMN IF NOT EXISTS nominal INT DEFAULT 1;
            ALTER TABLE IF EXISTS exchange_rates
                ALTER COLUMN nominal SET NOT NULL;
            ALTER TABLE IF EXISTS exchange_rates
                ALTER COLUMN nominal DROP DEFAULT;

            ALTER TABLE IF EXISTS exchange_rates
                ADD COLUMN IF NOT EXISTS vunit_rate DECIMAL(20,10) DEFAULT 0;
            ALTER TABLE IF EXISTS exchange_rates
                ALTER COLUMN vunit_rate SET NOT NULL;
            ALTER TABLE IF EXISTS exchange_rates
                ALTER COLUMN vunit_rate DROP DEFAULT;

            CREATE UNIQUE INDEX IF NOT EXISTS
                idx_exchange_rates_currency_date
            ON exchange_rates(currency_id, date);
        ";
    
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = new(createTablesSql, connection);
        await command.ExecuteNonQueryAsync();
    }
}