namespace AkilliYurt.API.Models;

public class GirisCikis
{
    public int KayitId { get; set; }
    public int KullaniciId { get; set; }
    public int CihazId { get; set; }
    public GecisYonu Yon { get; set; }
    public DateTime Zaman { get; set; } = DateTime.Now;

    public Kullanici? Kullanici { get; set; }
    public IotCihaz? Cihaz { get; set; }
}
