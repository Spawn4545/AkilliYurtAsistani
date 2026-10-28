using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YemekMenusuController : ControllerBase
{
    private static readonly string[] Ogunler = { "Kahvaltı", "Akşam" };
    private readonly AppDbContext _db;
    public YemekMenusuController(AppDbContext db) => _db = db;

    // GET api/yemekmenusu?baslangic=2026-11-02&gun=7   (varsayılan: bugünden itibaren 7 gün)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuDto>>> Listele([FromQuery] DateOnly? baslangic, [FromQuery] int gun = 7)
    {
        var bas = baslangic ?? DateOnly.FromDateTime(DateTime.Today);
        var bit = bas.AddDays(Math.Clamp(gun, 1, 31));

        return Ok(await _db.YemekMenuleri
            .Where(m => m.Tarih >= bas && m.Tarih < bit)
            .OrderBy(m => m.Tarih).ThenBy(m => m.Ogun)
            .Select(m => new MenuDto(m.MenuId, m.Tarih, m.Ogun, m.Yemekler, m.Kalori,
                m.Degerlendirmeler.Average(d => (double?)d.Puan),
                m.Degerlendirmeler.Count))
            .ToListAsync());
    }

    // POST api/yemekmenusu
    [HttpPost]
    public async Task<IActionResult> Ekle(MenuOlusturDto dto)
    {
        if (!Ogunler.Contains(dto.Ogun)) return BadRequest("Öğün 'Kahvaltı' veya 'Akşam' olmalıdır.");
        if (await _db.YemekMenuleri.AnyAsync(m => m.Tarih == dto.Tarih && m.Ogun == dto.Ogun))
            return Conflict("Bu tarih ve öğün için menü zaten girilmiş.");

        var menu = new YemekMenusu { Tarih = dto.Tarih, Ogun = dto.Ogun, Yemekler = dto.Yemekler, Kalori = dto.Kalori };
        _db.YemekMenuleri.Add(menu);
        await _db.SaveChangesAsync();
        return Ok(new { menu.MenuId });
    }

    // POST api/yemekmenusu/5/degerlendir
    [HttpPost("{menuId:int}/degerlendir")]
    public async Task<IActionResult> Degerlendir(int menuId, DegerlendirmeDto dto)
    {
        var menu = await _db.YemekMenuleri.FindAsync(menuId);
        if (menu is null) return NotFound("Menü bulunamadı.");
        if (menu.Tarih > DateOnly.FromDateTime(DateTime.Today))
            return BadRequest("Henüz servis edilmemiş bir öğün puanlanamaz.");

        var mevcut = await _db.YemekDegerlendirmeleri
            .FirstOrDefaultAsync(d => d.MenuId == menuId && d.KullaniciId == dto.KullaniciId);
        if (mevcut is null)
            _db.YemekDegerlendirmeleri.Add(new YemekDegerlendirme { MenuId = menuId, KullaniciId = dto.KullaniciId, Puan = dto.Puan, Yorum = dto.Yorum });
        else
        {
            mevcut.Puan = dto.Puan;           // aynı öğrenci tekrar puanlarsa güncellenir
            mevcut.Yorum = dto.Yorum;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
