namespace AkilliYurt.API.Models;

public class IotCihaz
{
    public int CihazId { get; set; }
    public int OdaId { get; set; }
    public string MacAdresi { get; set; } = string.Empty;
    public string CihazTuru { get; set; } = string.Empty;  // OrtamSensoru, RfidOkuyucu
    public DateTime? SonBaglanti { get; set; }

    // Eşik değerleri (yönetici tarafından ayarlanır)
    public double MaksSicaklik { get; set; } = 30;
    public int MaksGazSeviyesi { get; set; } = 300;

    public Oda? Oda { get; set; }
    public ICollection<SensorOlcum> Olcumler { get; set; } = new List<SensorOlcum>();
}
