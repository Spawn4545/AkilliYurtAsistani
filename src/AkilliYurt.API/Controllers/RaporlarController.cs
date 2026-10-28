using AkilliYurt.API.Data;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

// Yönetici paneli için raporlama ve analiz uç noktaları
[ApiController]
[Route("api/[controller]")]
public class RaporlarController : ControllerBase
{
    private readonly AppDbContext _db;
    public RaporlarController(AppDbContext db) => _db = db;

    // GET api/raporlar/ozet
    [HttpGet("ozet")]
    public async Task<IActionResult> Ozet()
    {
        var toplamKapasite = await _db.Odalar.SumAsync(o => o.Kapasite);
        var ogrenciSayisi = await _db.Kullanicilar.CountAsync(k => k.RolId == 1 && k.OdaId != null);
        var bugun = DateTime.Today;

        return Ok(new
        {
            OdaSayisi = await _db.Odalar.CountAsync(),
            ToplamKapasite = toplamKapasite,
            YerlesmisOgrenci = ogrenciSayisi,
            DolulukOrani = toplamKapasite == 0 ? 0 : Math.Round(100.0 * ogrenciSayisi / toplamKapasite, 1),
            AcikArizaTalebi = await _db.ArizaTalepleri.CountAsync(t => t.Durum != TalepDurum.Cozuldu),
            BugunGiris = await _db.GirisCikislar.CountAsync(g => g.Zaman >= bugun && g.Yon == GecisYonu.Giris),
            BugunAlarm = await _db.Bildirimler.Where(b => b.Tur == "Alarm" && b.Tarih >= bugun)
                .Select(b => b.Mesaj).Distinct().CountAsync()
        });
    }

    // GET api/raporlar/ariza?gun=30
    [HttpGet("ariza")]
    public async Task<IActionResult> ArizaRaporu([FromQuery] int gun = 30)
    {
        var sinir = DateTime.Now.AddDays(-gun);
        var talepler = await _db.ArizaTalepleri.Where(t => t.OlusturmaTarihi >= sinir).ToListAsync();
        var cozulenler = talepler.Where(t => t.CozumTarihi.HasValue).ToList();

        return Ok(new
        {
            ToplamTalep = talepler.Count,
            Cozulen = cozulenler.Count,
            OrtalamaCozumSaati = cozulenler.Count == 0 ? 0
                : Math.Round(cozulenler.Average(t => (t.CozumTarihi!.Value - t.OlusturmaTarihi).TotalHours), 1),
            KategoriyeGore = talepler.GroupBy(t => t.Kategori)
                .Select(g => new { Kategori = g.Key, Adet = g.Count() })
                .OrderByDescending(x => x.Adet)
        });
    }

    // GET api/raporlar/ortam/oda/5?saat=24  (grafik için saatlik ortalamalar)
    [HttpGet("ortam/oda/{odaId:int}")]
    public async Task<IActionResult> OrtamGecmisi(int odaId, [FromQuery] int saat = 24)
    {
        var sinir = DateTime.Now.AddHours(-Math.Clamp(saat, 1, 168));
        var olcumler = await _db.SensorOlcumleri
            .Where(o => o.Cihaz!.OdaId == odaId && o.OlcumZamani >= sinir)
            .Select(o => new { o.OlcumZamani, o.Sicaklik, o.Nem, o.GazSeviyesi })
            .ToListAsync();

        return Ok(olcumler
            .GroupBy(o => new DateTime(o.OlcumZamani.Year, o.OlcumZamani.Month, o.OlcumZamani.Day, o.OlcumZamani.Hour, 0, 0))
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Saat = g.Key,
                Sicaklik = Math.Round(g.Average(x => x.Sicaklik), 1),
                Nem = Math.Round(g.Average(x => x.Nem), 1),
                MaksGaz = g.Max(x => x.GazSeviyesi)
            }));
    }
}
