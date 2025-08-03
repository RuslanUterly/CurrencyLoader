using Npgsql;

namespace CurrencyLoader.Infrastructure;

public class DbInitializer
{
    private readonly NpgsqlDataSource _dataSource;
    
    public DbInitializer(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
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