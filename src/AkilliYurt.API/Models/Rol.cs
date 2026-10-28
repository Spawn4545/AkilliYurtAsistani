namespace AkilliYurt.API.Models;

public class Rol
{
    public int RolId { get; set; }
    public string RolAdi { get; set; } = string.Empty;   // Ogrenci, Yonetici, TeknikPersonel

    public ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
}
