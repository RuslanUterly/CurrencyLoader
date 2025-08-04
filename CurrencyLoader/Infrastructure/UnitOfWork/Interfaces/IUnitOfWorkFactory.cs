namespace CurrencyLoader.Infrastructure.UnitOfWork.Interfaces;

/// <summary>
/// Factory interface for creating instances of <see cref="IUnitOfWork"/>.
/// Ensures that a properly initialized unit of work is provided, typically with an open connection.
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Asynchronously creates a new <see cref="IUnitOfWork"/> instance.
    /// </summary>
    /// <param name="ct">Canceling an operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the <see cref="IUnitOfWork"/>.</returns>
    Task<IUnitOfWork> CreateAsync(CancellationToken ct);
}