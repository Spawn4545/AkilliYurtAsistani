namespace AkilliYurt.API.Models;

public class Bildirim
{
    public int BildirimId { get; set; }
    public int KullaniciId { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public string Tur { get; set; } = string.Empty;       // Talep, Duyuru, Alarm, Rezervasyon
    public bool OkunduMu { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;

    public Kullanici? Kullanici { get; set; }
}
