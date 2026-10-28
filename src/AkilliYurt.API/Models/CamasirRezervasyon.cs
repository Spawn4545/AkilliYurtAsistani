namespace AkilliYurt.API.Models;

public class CamasirRezervasyon
{
    public int RezervasyonId { get; set; }
    public int KullaniciId { get; set; }
    public int MakineId { get; set; }
    public DateTime BaslangicZamani { get; set; }
    public DateTime BitisZamani { get; set; }

    public Kullanici? Kullanici { get; set; }
    public CamasirMakinesi? Makine { get; set; }
}
