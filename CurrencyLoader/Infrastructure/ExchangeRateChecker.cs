using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Infrastructure.Interfaces.Repository;
using CurrencyLoader.Infrastructure.Repository;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;

namespace CurrencyLoader.Infrastructure;

public class ExchangeRateChecker : IExchangeRateChecker
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ExchangeRateChecker(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }
    
    public async Task<bool> CheckAsync(DateTime date, CancellationToken ct)
    {
        await using IUnitOfWork unitOfWork = await _unitOfWorkFactory.CreateAsync(ct);
        IExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository(unitOfWork);
        
        bool result = await exchangeRateRepository.ExistsByDateAsync(date, ct);
        return result;
    }
}