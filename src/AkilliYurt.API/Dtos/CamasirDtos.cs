using AkilliYurt.API.Models;

namespace AkilliYurt.API.Dtos;

public record MakineDto(int MakineId, string MakineNo, MakineDurum Durum, bool SuAnKullanimda);

public record RezervasyonOlusturDto(int KullaniciId, int MakineId, DateTime BaslangicZamani);

public record RezervasyonDto(int RezervasyonId, string MakineNo, DateTime BaslangicZamani, DateTime BitisZamani);
