using System.ComponentModel.DataAnnotations;

namespace AkilliYurt.API.Dtos;

public record BinaOlusturDto(
    [Required, StringLength(50)] string BinaAdi,
    [Range(1, 50)] int KatSayisi);

public record OdaOlusturDto(
    int BinaId,
    [Required, StringLength(10)] string OdaNo,
    [Range(0, 50)] int Kat,
    [Range(1, 10)] int Kapasite);

public record OdaDto(int OdaId, string BinaAdi, string OdaNo, int Kat, int Kapasite, int DoluYatak);

public record OgrenciYerlestirDto(int KullaniciId);
