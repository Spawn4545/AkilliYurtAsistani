using System.ComponentModel.DataAnnotations;

namespace AkilliYurt.API.Dtos;

public record MenuOlusturDto(
    DateOnly Tarih,
    [Required] string Ogun,
    [Required] string Yemekler,
    [Range(0, 5000)] int Kalori);

public record MenuDto(int MenuId, DateOnly Tarih, string Ogun, string Yemekler, int Kalori, double? OrtalamaPuan, int DegerlendirmeSayisi);

public record DegerlendirmeDto(
    int KullaniciId,
    [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")] int Puan,
    [StringLength(300)] string? Yorum);
