namespace AkilliYurt.API.Models;

public class Oda
{
    public int OdaId { get; set; }
    public int BinaId { get; set; }
    public string OdaNo { get; set; } = string.Empty;
    public int Kat { get; set; }
    public int Kapasite { get; set; }

    public Bina? Bina { get; set; }
    public ICollection<Kullanici> Ogrenciler { get; set; } = new List<Kullanici>();
    public ICollection<IotCihaz> Cihazlar { get; set; } = new List<IotCihaz>();
}
