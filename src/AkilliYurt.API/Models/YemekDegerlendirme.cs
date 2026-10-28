namespace AkilliYurt.API.Models;

public class YemekDegerlendirme
{
    public int DegerlendirmeId { get; set; }
    public int KullaniciId { get; set; }
    public int MenuId { get; set; }
    public int Puan { get; set; }                         // 1-5
    public string? Yorum { get; set; }

    public Kullanici? Kullanici { get; set; }
    public YemekMenusu? Menu { get; set; }
}
