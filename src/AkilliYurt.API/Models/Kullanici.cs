namespace AkilliYurt.API.Models;

public class Kullanici
{
    public int KullaniciId { get; set; }
    public int RolId { get; set; }
    public int? OdaId { get; set; }                       // yönetici/personel için boş
    public string AdSoyad { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string SifreHash { get; set; } = string.Empty;
    public string? RfidKartNo { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.Now;

    public Rol? Rol { get; set; }
    public Oda? Oda { get; set; }
    public ICollection<ArizaTalebi> ArizaTalepleri { get; set; } = new List<ArizaTalebi>();
    public ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();
}
