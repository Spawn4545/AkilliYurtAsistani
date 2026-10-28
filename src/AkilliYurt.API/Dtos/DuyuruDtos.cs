using System.ComponentModel.DataAnnotations;

namespace AkilliYurt.API.Dtos;

public record DuyuruOlusturDto(
    int YayinlayanId,
    [Required, StringLength(150)] string Baslik,
    [Required] string Icerik);

public record DuyuruDto(int DuyuruId, string Baslik, string Icerik, string Yayinlayan, DateTime YayinTarihi);

public record BildirimDto(int BildirimId, string Mesaj, string Tur, bool OkunduMu, DateTime Tarih);
