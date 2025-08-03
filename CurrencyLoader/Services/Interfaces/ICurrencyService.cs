using CurrencyLoader.Models;

namespace CurrencyLoader.Services.Interfaces;

public interface ICurrencyService
{
    Task<ValCurs?> GetExchangeRatesAsync(DateTime date);
}