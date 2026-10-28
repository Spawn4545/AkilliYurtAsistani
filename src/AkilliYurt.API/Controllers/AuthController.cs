using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using AkilliYurt.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuthController(AppDbContext db) => _db = db;

    // POST api/auth/kayit
    [HttpPost("kayit")]
    public async Task<ActionResult<KullaniciDto>> Kayit(KayitDto dto)
    {
        var eposta = dto.Eposta.Trim().ToLowerInvariant();
        if (await _db.Kullanicilar.AnyAsync(k => k.Eposta == eposta))
            return Conflict("Bu e-posta adresi zaten kayıtlı.");
        if (!await _db.Roller.AnyAsync(r => r.RolId == dto.RolId))
            return BadRequest("Geçersiz rol.");

        var kullanici = new Kullanici
        {
            AdSoyad = dto.AdSoyad.Trim(),
            Eposta = eposta,
            SifreHash = SifreYardimcisi.Hashle(dto.Sifre),
            RolId = dto.RolId
        };
        _db.Kullanicilar.Add(kullanici);
        await _db.SaveChangesAsync();

        return Ok(await DtoGetir(kullanici.KullaniciId));
    }

    // POST api/auth/giris
    [HttpPost("giris")]
    public async Task<ActionResult<KullaniciDto>> Giris(GirisDto dto)
    {
        var eposta = dto.Eposta.Trim().ToLowerInvariant();
        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta == eposta);
        if (kullanici is null || !SifreYardimcisi.Dogrula(dto.Sifre, kullanici.SifreHash))
            return Unauthorized("E-posta veya şifre hatalı.");

        return Ok(await DtoGetir(kullanici.KullaniciId));
    }

    private Task<KullaniciDto> DtoGetir(int id) =>
        _db.Kullanicilar.Where(k => k.KullaniciId == id)
            .Select(k => new KullaniciDto(k.KullaniciId, k.AdSoyad, k.Eposta, k.Rol!.RolAdi, k.OdaId))
            .FirstAsync();
}
