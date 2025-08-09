using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CurrencyLoader.Services;

/// <summary>
/// Quartz job that imports exchange rates for a defined date range (by default: the last month).
/// </summary>
public class ExchangeImportJob : IJob
{
    private readonly IExchangeRateImporter _importer;
    private readonly ILogger<ExchangeImportJob> _logger;

    public ExchangeImportJob(IExchangeRateImporter importer, ILogger<ExchangeImportJob> logger)
    {
        _importer = importer;
        _logger = logger;
    }

    /// <summary>
    /// Executes the job: calculates the date range (last month) and triggers the import operation.
    /// </summary>
    /// <param name="context">The Quartz job execution context provided by the scheduler.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous execution of the job.</returns>
    /// <remarks>
    /// The method logs the start and end times of the import. It uses <see cref="CancellationToken.None"/>
    /// when calling <see cref="IExchangeRateImporter.ImportAsync"/>. Any exceptions raised during import
    /// are caught and logged via <see cref="ILogger.LogError"/>; they are not propagated further.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate.AddMonths(-1);
            
            _logger.LogInformation("Import starting for {Date}", DateTime.Now);
            
            await _importer.ImportAsync(startDate, endDate, CancellationToken.None);
            
            _logger.LogInformation("Import completed for {Date}", DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Quartz job failed");
        }
    }
}