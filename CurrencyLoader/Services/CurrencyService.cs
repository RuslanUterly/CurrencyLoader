using System.Text;
using System.Xml.Serialization;
using CurrencyLoader.Models;
using CurrencyLoader.Models.Options;
using CurrencyLoader.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CurrencyLoader.Services;

/// <summary>
/// Service that retrieves exchange rates (ValCurs) from the Central Bank (CBR) HTTP API.
/// </summary>
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
    
    /// <summary>
    /// Retrieves exchange rates for the specified date.
    /// </summary>
    /// <param name="date">The date for which exchange rates should be retrieved.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to cancel the operation. Defaults to <c>default</c>.</param>
    /// <returns>
    /// A task that returns a <see cref="ValCurs"/> instance containing exchange rates for the requested date,
    /// or <c>null</c> if the data could not be retrieved or parsed.
    /// </returns>
    /// <remarks>
    /// The method formats <paramref name="date"/> as <c>dd/MM/yyyy</c> and substitutes it into the configured
    /// <see cref="CbrOptions.BaseUrl"/> (which is expected to contain a <c>{date}</c> placeholder). It performs
    /// an HTTP GET request, expects the response encoded in Windows-1251, and deserializes the XML into <see cref="ValCurs"/>.
    /// Any non-success HTTP status, deserialization error, or other exception will be logged and the method will return <c>null</c>.
    /// </remarks>
    public async Task<ValCurs?> GetExchangeRatesAsync(DateTime date, CancellationToken ct = default)
    {
        try
        {
            string formattedDate = date.ToString("dd/MM/yyyy");
            HttpClient client = _clientFactory.CreateClient();
            
            HttpResponseMessage response = await client.GetAsync(
                $"{_options.Value.BaseUrl.Replace("{date}", formattedDate)}", ct);
        
            if (!response.IsSuccessStatusCode) return null;
        
            // Decode the byte array using Windows-1251 encoding (required for CBR XML)
            var data = await response.Content.ReadAsByteArrayAsync(ct);
            var xmlContent = Encoding.GetEncoding("windows-1251").GetString(data);
            
            return ParseXmlResponse(xmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data for {Date}", date);
            return null;
        }
    }
    
    /// <summary>
    /// Parses XML content into a <see cref="ValCurs"/> object.
    /// </summary>
    /// <param name="xmlContent">The XML string to deserialize.</param>
    /// <returns>
    /// A <see cref="ValCurs"/> instance if deserialization succeeds; otherwise <c>null</c>.
    /// </returns>
    private ValCurs? ParseXmlResponse(string xmlContent)
    {
        XmlSerializer serializer = new(typeof(ValCurs));
        using StringReader reader = new(xmlContent);
        return (ValCurs?)serializer.Deserialize(reader);
    }
}