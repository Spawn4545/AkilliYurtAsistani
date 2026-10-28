namespace AkilliYurt.API.Models;

public class Bina
{
    public int BinaId { get; set; }
    public string BinaAdi { get; set; } = string.Empty;
    public int KatSayisi { get; set; }

    public ICollection<Oda> Odalar { get; set; } = new List<Oda>();
    public ICollection<CamasirMakinesi> CamasirMakineleri { get; set; } = new List<CamasirMakinesi>();
}
