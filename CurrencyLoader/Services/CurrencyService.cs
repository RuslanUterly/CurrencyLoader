using System.Text;
using System.Xml.Serialization;
using CurrencyLoader.Models;
using CurrencyLoader.Models.Options;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CurrencyLoader.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IOptions<CbrOptions> _options;
    private readonly ILogger<CurrencyService> _logger;
    
    public CurrencyService(IOptions<CbrOptions> options, ILogger<CurrencyService> logger, IHttpClientFactory clientFactory)
    {
        _options = options;
        _logger = logger;
        _clientFactory = clientFactory;
    }
    
    public async Task<ValCurs?> GetExchangeRatesAsync(DateTime date, CancellationToken ct = default)
    {
        try
        {
            string formattedDate = date.ToString("dd/MM/yyyy");
            HttpClient client = _clientFactory.CreateClient();
            
            HttpResponseMessage response = await client.GetAsync(
                $"{_options.Value.BaseUrl.Replace("{date}", formattedDate)}");
        
            if (!response.IsSuccessStatusCode) return null;
        
            var data = await response.Content.ReadAsByteArrayAsync();
            var xmlContent = Encoding.GetEncoding("windows-1251").GetString(data);
            return ParseXmlResponse(xmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data for {Date}", date);
            return null;
        }
    }

    private ValCurs? ParseXmlResponse(string xmlContent)
    {
        XmlSerializer serializer = new(typeof(ValCurs));
        using StringReader reader = new(xmlContent);
        return (ValCurs?)serializer.Deserialize(reader);
    }
}