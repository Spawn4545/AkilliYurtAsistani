using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DuyurularController : ControllerBase
{
    private readonly AppDbContext _db;
    public DuyurularController(AppDbContext db) => _db = db;

    // GET api/duyurular
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DuyuruDto>>> Listele() =>
        Ok(await _db.Duyurular
            .OrderByDescending(d => d.YayinTarihi)
            .Select(d => new DuyuruDto(d.DuyuruId, d.Baslik, d.Icerik, d.Yayinlayan!.AdSoyad, d.YayinTarihi))
            .ToListAsync());

    // POST api/duyurular  (sadece yönetici)
    [HttpPost]
    public async Task<IActionResult> Yayinla(DuyuruOlusturDto dto)
    {
        var yonetici = await _db.Kullanicilar.FindAsync(dto.YayinlayanId);
        if (yonetici is null || yonetici.RolId != 2)
            return BadRequest("Duyuruyu sadece yöneticiler yayınlayabilir.");

        var duyuru = new Duyuru { YayinlayanId = yonetici.KullaniciId, Baslik = dto.Baslik, Icerik = dto.Icerik };
        _db.Duyurular.Add(duyuru);

        // Tüm öğrencilere bildirim gönder
        var ogrenciler = await _db.Kullanicilar.Where(k => k.RolId == 1).Select(k => k.KullaniciId).ToListAsync();
        foreach (var id in ogrenciler)
            _db.Bildirimler.Add(new Bildirim { KullaniciId = id, Tur = "Duyuru", Mesaj = $"Yeni duyuru: {dto.Baslik}" });

        await _db.SaveChangesAsync();
        return Ok(new { duyuru.DuyuruId });
    }

    // DELETE api/duyurular/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Sil(int id)
    {
        var duyuru = await _db.Duyurular.FindAsync(id);
        if (duyuru is null) return NotFound();
        _db.Duyurular.Remove(duyuru);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
