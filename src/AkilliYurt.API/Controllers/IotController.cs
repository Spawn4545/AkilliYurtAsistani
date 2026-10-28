using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IotController : ControllerBase
{
    private readonly AppDbContext _db;
    public IotController(AppDbContext db) => _db = db;

    // POST api/iot/olcum  (UC-02: ESP32 ölçüm gönderir, eşik aşılırsa alarm)
    [HttpPost("olcum")]
    public async Task<ActionResult<OlcumCevapDto>> OlcumAl(OlcumGonderDto dto)
    {
        var cihaz = await _db.IotCihazlar.FirstOrDefaultAsync(c => c.MacAdresi == dto.MacAdresi);
        if (cihaz is null) return Unauthorized("Kayıtsız cihaz.");

        cihaz.SonBaglanti = DateTime.Now;
        _db.SensorOlcumleri.Add(new SensorOlcum
        {
            CihazId = cihaz.CihazId,
            Sicaklik = dto.Sicaklik,
            Nem = dto.Nem,
            GazSeviyesi = dto.GazSeviyesi
        });

        var sorunlar = new List<string>();
        if (dto.GazSeviyesi > cihaz.MaksGazSeviyesi) sorunlar.Add($"gaz seviyesi yüksek ({dto.GazSeviyesi} ppm)");
        if (dto.Sicaklik > cihaz.MaksSicaklik) sorunlar.Add($"sıcaklık yüksek ({dto.Sicaklik:0.0} °C)");

        var alarm = sorunlar.Count > 0;
        if (alarm && !await SonOnDakikadaAlarmVarMi(cihaz.OdaId))
        {
            var mesaj = $"ALARM - Oda {cihaz.OdaId}: {string.Join(", ", sorunlar)}";
            var alicilar = await _db.Kullanicilar
                .Where(k => k.RolId == 2 || k.OdaId == cihaz.OdaId)   // yöneticiler + oda sakinleri
                .Select(k => k.KullaniciId).ToListAsync();
            foreach (var id in alicilar)
                _db.Bildirimler.Add(new Bildirim { KullaniciId = id, Tur = "Alarm", Mesaj = mesaj });
        }

        await _db.SaveChangesAsync();
        return Ok(new OlcumCevapDto(alarm, alarm ? string.Join(", ", sorunlar) : "Normal"));
    }

    // GET api/iot/oda/5/son-olcum
    [HttpGet("oda/{odaId:int}/son-olcum")]
    public async Task<IActionResult> SonOlcum(int odaId)
    {
        var olcum = await _db.SensorOlcumleri
            .Where(o => o.Cihaz!.OdaId == odaId)
            .OrderByDescending(o => o.OlcumZamani)
            .Select(o => new { o.Sicaklik, o.Nem, o.GazSeviyesi, o.OlcumZamani })
            .FirstOrDefaultAsync();
        return olcum is null ? NotFound("Bu oda için ölçüm yok.") : Ok(olcum);
    }

    // POST api/iot/kart  (RFID kart okutuldu → giriş/çıkış kaydı)
    [HttpPost("kart")]
    public async Task<IActionResult> KartOkut(KartOkutDto dto)
    {
        var cihaz = await _db.IotCihazlar.FirstOrDefaultAsync(c => c.MacAdresi == dto.MacAdresi);
        if (cihaz is null) return Unauthorized("Kayıtsız cihaz.");

        var kullanici = await _db.Kullanicilar.FirstOrDefaultAsync(k => k.RfidKartNo == dto.KartNo);
        if (kullanici is null) return NotFound("Kart tanımlı değil.");

        // Son kayıt giriş ise bu okutma çıkıştır, değilse giriştir
        var sonYon = await _db.GirisCikislar
            .Where(g => g.KullaniciId == kullanici.KullaniciId)
            .OrderByDescending(g => g.Zaman)
            .Select(g => (GecisYonu?)g.Yon)
            .FirstOrDefaultAsync();
        var yon = sonYon == GecisYonu.Giris ? GecisYonu.Cikis : GecisYonu.Giris;

        _db.GirisCikislar.Add(new GirisCikis { KullaniciId = kullanici.KullaniciId, CihazId = cihaz.CihazId, Yon = yon });
        cihaz.SonBaglanti = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(new { kullanici.AdSoyad, Yon = yon.ToString() });
    }

    // Aynı oda için 10 dakika içinde tekrar alarm bildirimi gönderilmesin
    private Task<bool> SonOnDakikadaAlarmVarMi(int odaId)
    {
        var sinir = DateTime.Now.AddMinutes(-10);
        var aranan = $"ALARM - Oda {odaId}:";
        return _db.Bildirimler.AnyAsync(b => b.Tur == "Alarm" && b.Tarih > sinir && b.Mesaj.StartsWith(aranan));
    }
}
