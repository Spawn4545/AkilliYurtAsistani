using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OdalarController : ControllerBase
{
    private readonly AppDbContext _db;
    public OdalarController(AppDbContext db) => _db = db;

    // GET api/odalar?binaId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OdaDto>>> Listele([FromQuery] int? binaId)
    {
        var sorgu = _db.Odalar.AsQueryable();
        if (binaId.HasValue)
            sorgu = sorgu.Where(o => o.BinaId == binaId);

        var odalar = await sorgu
            .OrderBy(o => o.OdaNo)
            .Select(o => new OdaDto(o.OdaId, o.Bina!.BinaAdi, o.OdaNo, o.Kat, o.Kapasite, o.Ogrenciler.Count))
            .ToListAsync();
        return Ok(odalar);
    }

    // GET api/odalar/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OdaDto>> Getir(int id)
    {
        var oda = await _db.Odalar
            .Where(o => o.OdaId == id)
            .Select(o => new OdaDto(o.OdaId, o.Bina!.BinaAdi, o.OdaNo, o.Kat, o.Kapasite, o.Ogrenciler.Count))
            .FirstOrDefaultAsync();
        return oda is null ? NotFound() : Ok(oda);
    }

    // POST api/odalar/bina
    [HttpPost("bina")]
    public async Task<IActionResult> BinaEkle(BinaOlusturDto dto)
    {
        var bina = new Bina { BinaAdi = dto.BinaAdi, KatSayisi = dto.KatSayisi };
        _db.Binalar.Add(bina);
        await _db.SaveChangesAsync();
        return Ok(new { bina.BinaId });
    }

    // POST api/odalar
    [HttpPost]
    public async Task<IActionResult> OdaEkle(OdaOlusturDto dto)
    {
        if (!await _db.Binalar.AnyAsync(b => b.BinaId == dto.BinaId))
            return BadRequest("Bina bulunamadı.");
        if (await _db.Odalar.AnyAsync(o => o.BinaId == dto.BinaId && o.OdaNo == dto.OdaNo))
            return Conflict("Bu binada aynı numaralı oda zaten var.");

        var oda = new Oda { BinaId = dto.BinaId, OdaNo = dto.OdaNo, Kat = dto.Kat, Kapasite = dto.Kapasite };
        _db.Odalar.Add(oda);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Getir), new { id = oda.OdaId }, new { oda.OdaId });
    }

    // POST api/odalar/5/yerlestir
    [HttpPost("{id:int}/yerlestir")]
    public async Task<IActionResult> OgrenciYerlestir(int id, OgrenciYerlestirDto dto)
    {
        var oda = await _db.Odalar.Include(o => o.Ogrenciler).FirstOrDefaultAsync(o => o.OdaId == id);
        if (oda is null) return NotFound("Oda bulunamadı.");
        if (oda.Ogrenciler.Count >= oda.Kapasite) return BadRequest("Oda dolu.");

        var ogrenci = await _db.Kullanicilar.FindAsync(dto.KullaniciId);
        if (ogrenci is null) return NotFound("Öğrenci bulunamadı.");
        if (ogrenci.RolId != 1) return BadRequest("Sadece öğrenciler odaya yerleştirilebilir.");

        ogrenci.OdaId = id;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
