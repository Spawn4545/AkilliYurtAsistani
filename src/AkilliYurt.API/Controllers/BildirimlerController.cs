using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BildirimlerController : ControllerBase
{
    private readonly AppDbContext _db;
    public BildirimlerController(AppDbContext db) => _db = db;

    // GET api/bildirimler/kullanici/3?sadeceOkunmamis=true
    [HttpGet("kullanici/{kullaniciId:int}")]
    public async Task<ActionResult<IEnumerable<BildirimDto>>> Listele(int kullaniciId, [FromQuery] bool sadeceOkunmamis = false)
    {
        var sorgu = _db.Bildirimler.Where(b => b.KullaniciId == kullaniciId);
        if (sadeceOkunmamis) sorgu = sorgu.Where(b => !b.OkunduMu);

        return Ok(await sorgu
            .OrderByDescending(b => b.Tarih)
            .Take(50)
            .Select(b => new BildirimDto(b.BildirimId, b.Mesaj, b.Tur, b.OkunduMu, b.Tarih))
            .ToListAsync());
    }

    // PUT api/bildirimler/5/okundu
    [HttpPut("{id:int}/okundu")]
    public async Task<IActionResult> OkunduIsaretle(int id)
    {
        var bildirim = await _db.Bildirimler.FindAsync(id);
        if (bildirim is null) return NotFound();
        bildirim.OkunduMu = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
