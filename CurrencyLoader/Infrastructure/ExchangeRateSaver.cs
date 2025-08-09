using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Infrastructure.Interfaces.Repository;
using CurrencyLoader.Infrastructure.Repository;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using CurrencyLoader.Models;

namespace CurrencyLoader.Infrastructure;

/// <summary>
/// Service for storing exchange rates (ValCurs) into the database using a unit-of-work and repository pattern.
/// </summary>
public class ExchangeRateSaver : IExchangeRateSaver
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ExchangeRateSaver(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    /// <summary>
    /// Saves exchange rates from the provided <see cref="ValCurs"/> data for the specified date.
    /// </summary>
    /// <param name="data">The <see cref="ValCurs"/> object containing currency entries to persist.</param>
    /// <param name="date">The date associated with the exchange rates being saved.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the operation. Defaults to <c>default</c>.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns>
    /// <remarks>
    /// The method:
    /// <div>1. Creates a unit of work and begins a transaction.</div>
    /// <div>2. Iterates over <see cref="ValCurs.Valutes"/>. For each <see cref="Valute"/>, it attempts to find the
    /// currency ID by code via <c>ICurrencyRepository.GetIdByCodeAsync</c>. If the currency does not exist,
    /// it is added using <c>ICurrencyRepository.AddAsync</c>.</div>
    /// <div>3. Adds an exchange rate entry via <c>IExchangeRateRepository.AddAsync</c> for each currency.</div>
    /// <div>4. Commits the transaction when all entries are successfully processed.</div>
    ///
    /// If an exception occurs, the transaction is rolled back and the exception is rethrown.
    /// </remarks>
    public async Task SaveAsync(ValCurs data, DateTime date, CancellationToken ct = default)
    {
        await using IUnitOfWork unitOfWork = await _unitOfWorkFactory.CreateAsync(ct);
        ICurrencyRepository currencyRepository = new CurrencyRepository(unitOfWork);
        IExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository(unitOfWork);

        try
        {
            await unitOfWork.BeginTransactionAsync(ct);

            foreach (Valute valute in data.Valutes)
            {
                // Get the currency ID by code or add a new one if it is not found
                int id = await currencyRepository.GetIdByCodeAsync(valute.CharCode, ct)
                         ?? await currencyRepository.AddAsync(valute.CharCode, valute.Name, ct);

                await exchangeRateRepository.AddAsync(id, date, valute.Nominal, valute.Value, valute.VunitRate, ct);
            }

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
