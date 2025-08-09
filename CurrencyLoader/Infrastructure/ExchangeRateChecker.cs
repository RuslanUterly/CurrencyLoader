using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Infrastructure.Interfaces.Repository;
using CurrencyLoader.Infrastructure.Repository;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;

namespace CurrencyLoader.Infrastructure;

/// <summary>
/// Service for checking whether exchange rates for a specific date already exist in the database.
/// </summary>
public class ExchangeRateChecker : IExchangeRateChecker
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ExchangeRateChecker(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }
    
    /// <summary>
    /// Checks if exchange rates for the specified date are already stored.
    /// </summary>
    /// <param name="date">The date to check for existing exchange rates.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to cancel the operation. Defaults to <c>default</c>.</param>
    /// <returns>
    /// A task that returns <c>true</c> if the exchange rates for the specified date exist; otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The method creates a unit of work using <see cref="_unitOfWorkFactory"/>, constructs an
    /// <c>ExchangeRateRepository</c> and delegates the existence check to the repository's
    /// <c>ExistsByDateAsync</c> method. The unit of work is disposed after the check (see usage of <c>await using</c>).
    /// </remarks>
    public async Task<bool> CheckAsync(DateTime date, CancellationToken ct = default)
    {
        await using IUnitOfWork unitOfWork = await _unitOfWorkFactory.CreateAsync(ct);
        IExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository(unitOfWork);
        
        bool result = await exchangeRateRepository.ExistsByDateAsync(date, ct);
        return result;
    }
}