using System.ComponentModel.DataAnnotations;

namespace AkilliYurt.API.Dtos;

public record KayitDto(
    [Required, StringLength(100)] string AdSoyad,
    [Required, EmailAddress] string Eposta,
    [Required, MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")] string Sifre,
    int RolId = 1);

public record GirisDto([Required, EmailAddress] string Eposta, [Required] string Sifre);

public record KullaniciDto(int KullaniciId, string AdSoyad, string Eposta, string Rol, int? OdaId);
