namespace CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> CreateAsync(CancellationToken ct);
}