namespace AkilliYurt.API.Models;

public class Duyuru
{
    public int DuyuruId { get; set; }
    public int YayinlayanId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Icerik { get; set; } = string.Empty;
    public DateTime YayinTarihi { get; set; } = DateTime.Now;

    public Kullanici? Yayinlayan { get; set; }
}
