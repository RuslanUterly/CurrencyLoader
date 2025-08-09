using System.Globalization;
using System.Xml.Serialization;

namespace CurrencyLoader.Models;

public class Valute
{
    [XmlAttribute("ID")]
    public string Id { get; set; }
    
    [XmlElement("NumCode")]
    public string NumCode { get; set; }
    
    [XmlElement("CharCode")]
    public string CharCode { get; set; }
    
    [XmlElement("Nominal")]
    public int Nominal { get; set; }
    
    [XmlElement("Name")]
    public string Name { get; set; }
    
    [XmlElement("Value")]
    public string ValueString { get; set; }
    
    [XmlElement("VunitRate")]
    public string VunitRateString { get; set; }
    
    [XmlIgnore]
    public decimal Value
    {
        get
        {
            return decimal.Parse(ValueString, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"));
        }
        set
        {
            ValueString = value.ToString(CultureInfo.GetCultureInfo("ru-RU"));
        }
    }
    
    [XmlIgnore]
    public decimal VunitRate
    {
        get
        {
            // Exponential notation processing (2,22616E-05)
            if (VunitRateString.Contains("E") || VunitRateString.Contains("e"))
            {
                if (double.TryParse(
                        VunitRateString.Replace(',', '.'), 
                        NumberStyles.Any, 
                        CultureInfo.InvariantCulture, 
                        out double result))
                {
                    return (decimal)result;
                }
            }
            
            // Regular format processing
            return decimal.Parse(
                VunitRateString, 
                NumberStyles.Any, 
                CultureInfo.GetCultureInfo("ru-RU"));
        }
    }
}