using System.ComponentModel.DataAnnotations;
using AkilliYurt.API.Models;

namespace AkilliYurt.API.Dtos;

public record ArizaOlusturDto(
    int KullaniciId,
    [Required] string Kategori,
    [Required, MinLength(10, ErrorMessage = "Açıklama en az 10 karakter olmalıdır.")] string Aciklama,
    string? FotografYolu);

public record PersonelAtaDto(int PersonelId);

public record ArizaDto(
    int TalepId, string OgrenciAdi, string OdaNo, string Kategori, string Aciklama,
    TalepDurum Durum, string? AtananPersonel, DateTime OlusturmaTarihi, DateTime? CozumTarihi);
