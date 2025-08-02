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
    public string Value { get; set; }
}