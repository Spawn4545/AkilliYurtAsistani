namespace AkilliYurt.API.Models;

public class CamasirMakinesi
{
    public int MakineId { get; set; }
    public int BinaId { get; set; }
    public string MakineNo { get; set; } = string.Empty;
    public MakineDurum Durum { get; set; } = MakineDurum.Bos;

    public Bina? Bina { get; set; }
    public ICollection<CamasirRezervasyon> Rezervasyonlar { get; set; } = new List<CamasirRezervasyon>();
}
