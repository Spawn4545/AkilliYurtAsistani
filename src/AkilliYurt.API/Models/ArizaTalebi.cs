namespace AkilliYurt.API.Models;

public class ArizaTalebi
{
    public int TalepId { get; set; }
    public int KullaniciId { get; set; }
    public int OdaId { get; set; }
    public int? AtananPersonelId { get; set; }
    public string Kategori { get; set; } = string.Empty;  // Elektrik, Su Tesisatı, Mobilya, İnternet, Diğer
    public string Aciklama { get; set; } = string.Empty;
    public string? FotografYolu { get; set; }
    public TalepDurum Durum { get; set; } = TalepDurum.Beklemede;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    public DateTime? CozumTarihi { get; set; }

    public Kullanici? Kullanici { get; set; }
    public Oda? Oda { get; set; }
    public Kullanici? AtananPersonel { get; set; }
}
