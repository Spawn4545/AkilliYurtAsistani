namespace AkilliYurt.API.Models;

public class YemekMenusu
{
    public int MenuId { get; set; }
    public DateOnly Tarih { get; set; }
    public string Ogun { get; set; } = string.Empty;      // Kahvaltı, Akşam
    public string Yemekler { get; set; } = string.Empty;
    public int Kalori { get; set; }

    public ICollection<YemekDegerlendirme> Degerlendirmeler { get; set; } = new List<YemekDegerlendirme>();
}
