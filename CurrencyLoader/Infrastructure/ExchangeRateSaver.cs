using CurrencyLoader.Infrastructure.Interfaces;
using CurrencyLoader.Infrastructure.Interfaces.Repository;
using CurrencyLoader.Infrastructure.Repository;
using CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;
using CurrencyLoader.Models;

namespace CurrencyLoader.Infrastructure;

public class ExchangeRateSaver : IExchangeRateSaver
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ExchangeRateSaver(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

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
