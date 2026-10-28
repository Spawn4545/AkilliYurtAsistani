using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

// UC-03: Çamaşır Makinesi Rezervasyonu
[ApiController]
[Route("api/[controller]")]
public class CamasirhaneController : ControllerBase
{
    private static readonly TimeSpan SlotSuresi = TimeSpan.FromHours(1);
    private readonly AppDbContext _db;
    public CamasirhaneController(AppDbContext db) => _db = db;

    // GET api/camasirhane/bina/1/makineler
    [HttpGet("bina/{binaId:int}/makineler")]
    public async Task<ActionResult<IEnumerable<MakineDto>>> Makineler(int binaId)
    {
        var simdi = DateTime.Now;
        return Ok(await _db.CamasirMakineleri
            .Where(m => m.BinaId == binaId)
            .OrderBy(m => m.MakineNo)
            .Select(m => new MakineDto(m.MakineId, m.MakineNo, m.Durum,
                m.Rezervasyonlar.Any(r => r.BaslangicZamani <= simdi && r.BitisZamani > simdi)))
            .ToListAsync());
    }

    // GET api/camasirhane/makine/2/bos-saatler?tarih=2026-11-03
    [HttpGet("makine/{makineId:int}/bos-saatler")]
    public async Task<ActionResult<IEnumerable<DateTime>>> BosSaatler(int makineId, [FromQuery] DateOnly tarih)
    {
        var gunBasi = tarih.ToDateTime(new TimeOnly(8, 0));     // çamaşırhane 08:00 - 23:00 arası açık
        var gunSonu = tarih.ToDateTime(new TimeOnly(23, 0));

        var doluSaatler = await _db.CamasirRezervasyonlari
            .Where(r => r.MakineId == makineId && r.BaslangicZamani >= gunBasi && r.BaslangicZamani < gunSonu)
            .Select(r => r.BaslangicZamani)
            .ToListAsync();

        var bos = new List<DateTime>();
        for (var t = gunBasi; t < gunSonu; t += SlotSuresi)
            if (t > DateTime.Now && !doluSaatler.Contains(t))
                bos.Add(t);
        return Ok(bos);
    }

    // POST api/camasirhane/rezervasyon
    [HttpPost("rezervasyon")]
    public async Task<IActionResult> RezervasyonYap(RezervasyonOlusturDto dto)
    {
        var makine = await _db.CamasirMakineleri.FindAsync(dto.MakineId);
        if (makine is null) return NotFound("Makine bulunamadı.");
        if (makine.Durum == MakineDurum.Arizali) return BadRequest("Bu makine arızalı, rezervasyon yapılamaz.");

        var baslangic = new DateTime(dto.BaslangicZamani.Year, dto.BaslangicZamani.Month, dto.BaslangicZamani.Day,
            dto.BaslangicZamani.Hour, 0, 0);                  // saat başına yuvarla
        if (baslangic <= DateTime.Now) return BadRequest("Geçmiş bir saate rezervasyon yapılamaz.");

        var simdi = DateTime.Now;
        if (await _db.CamasirRezervasyonlari.AnyAsync(r => r.KullaniciId == dto.KullaniciId && r.BitisZamani > simdi))
            return BadRequest("Zaten aktif bir rezervasyonunuz var.");

        if (await _db.CamasirRezervasyonlari.AnyAsync(r => r.MakineId == dto.MakineId && r.BaslangicZamani == baslangic))
            return Conflict("Bu saat dolu.");

        var rez = new CamasirRezervasyon
        {
            KullaniciId = dto.KullaniciId,
            MakineId = dto.MakineId,
            BaslangicZamani = baslangic,
            BitisZamani = baslangic + SlotSuresi
        };
        _db.CamasirRezervasyonlari.Add(rez);
        await _db.SaveChangesAsync();
        return Ok(new RezervasyonDto(rez.RezervasyonId, makine.MakineNo, rez.BaslangicZamani, rez.BitisZamani));
    }

    // DELETE api/camasirhane/rezervasyon/5  (en geç 30 dk öncesine kadar iptal)
    [HttpDelete("rezervasyon/{id:int}")]
    public async Task<IActionResult> Iptal(int id)
    {
        var rez = await _db.CamasirRezervasyonlari.FindAsync(id);
        if (rez is null) return NotFound();
        if (rez.BaslangicZamani - DateTime.Now < TimeSpan.FromMinutes(30))
            return BadRequest("Rezervasyon en geç 30 dakika öncesine kadar iptal edilebilir.");

        _db.CamasirRezervasyonlari.Remove(rez);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
