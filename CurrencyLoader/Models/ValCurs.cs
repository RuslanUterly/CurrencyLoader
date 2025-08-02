using System.Xml.Serialization;

namespace CurrencyLoader.Models;

[XmlRoot("ValCurs")]
public class ValCurs
{
    [XmlAttribute("Date")]
    public string Date { get; set; }
    
    [XmlAttribute("name")]
    public string Name { get; set; }
    
    [XmlElement("Valute")]
    public List<Valute> Valutes { get; set; }
}