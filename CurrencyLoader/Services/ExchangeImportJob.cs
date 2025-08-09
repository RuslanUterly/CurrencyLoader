using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CurrencyLoader.Services;

public class ExchangeImportJob : IJob
{
    private readonly IExchangeRateImporter _importer;
    private readonly ILogger<ExchangeImportJob> _logger;

    public ExchangeImportJob(IExchangeRateImporter importer, ILogger<ExchangeImportJob> logger)
    {
        _importer = importer;
        _logger = logger;
    }

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