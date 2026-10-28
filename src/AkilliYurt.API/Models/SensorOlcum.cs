namespace AkilliYurt.API.Models;

public class SensorOlcum
{
    public int OlcumId { get; set; }
    public int CihazId { get; set; }
    public double Sicaklik { get; set; }
    public double Nem { get; set; }
    public int GazSeviyesi { get; set; }
    public DateTime OlcumZamani { get; set; } = DateTime.Now;

    public IotCihaz? Cihaz { get; set; }
}
