using AkilliYurt.API.Data;
using AkilliYurt.API.Dtos;
using AkilliYurt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArizaTalepleriController : ControllerBase
{
    private static readonly string[] Kategoriler = { "Elektrik", "Su Tesisatı", "Mobilya", "İnternet", "Diğer" };
    private readonly AppDbContext _db;
    public ArizaTalepleriController(AppDbContext db) => _db = db;

    // GET api/arizatalepleri?durum=Beklemede&kullaniciId=3
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArizaDto>>> Listele([FromQuery] TalepDurum? durum, [FromQuery] int? kullaniciId)
    {
        var sorgu = _db.ArizaTalepleri.AsQueryable();
        if (durum.HasValue) sorgu = sorgu.Where(t => t.Durum == durum);
        if (kullaniciId.HasValue) sorgu = sorgu.Where(t => t.KullaniciId == kullaniciId);

        var liste = await sorgu
            .OrderByDescending(t => t.OlusturmaTarihi)
            .Select(t => new ArizaDto(t.TalepId, t.Kullanici!.AdSoyad, t.Oda!.OdaNo, t.Kategori, t.Aciklama,
                t.Durum, t.AtananPersonel != null ? t.AtananPersonel.AdSoyad : null, t.OlusturmaTarihi, t.CozumTarihi))
            .ToListAsync();
        return Ok(liste);
    }

    // POST api/arizatalepleri  (UC-01: Arıza Talebi Oluşturma)
    [HttpPost]
    public async Task<IActionResult> Olustur(ArizaOlusturDto dto)
    {
        if (!Kategoriler.Contains(dto.Kategori))
            return BadRequest("Lütfen geçerli bir kategori seçiniz.");

        var ogrenci = await _db.Kullanicilar.FindAsync(dto.KullaniciId);
        if (ogrenci is null) return NotFound("Kullanıcı bulunamadı.");
        if (ogrenci.OdaId is null) return BadRequest("Öğrenci herhangi bir odaya kayıtlı değil.");

        var talep = new ArizaTalebi
        {
            KullaniciId = ogrenci.KullaniciId,
            OdaId = ogrenci.OdaId.Value,           // oda bilgisi otomatik doldurulur
            Kategori = dto.Kategori,
            Aciklama = dto.Aciklama,
            FotografYolu = dto.FotografYolu
        };
        _db.ArizaTalepleri.Add(talep);

        // Yöneticilere bildirim
        var yoneticiler = await _db.Kullanicilar.Where(k => k.RolId == 2).Select(k => k.KullaniciId).ToListAsync();
        foreach (var id in yoneticiler)
            _db.Bildirimler.Add(new Bildirim { KullaniciId = id, Tur = "Talep", Mesaj = $"Yeni arıza talebi: {dto.Kategori}" });

        await _db.SaveChangesAsync();
        return Ok(new { talep.TalepId, Mesaj = "Talebiniz alındı." });
    }

    // PUT api/arizatalepleri/5/ata
    [HttpPut("{id:int}/ata")]
    public async Task<IActionResult> PersoneleAta(int id, PersonelAtaDto dto)
    {
        var talep = await _db.ArizaTalepleri.FindAsync(id);
        if (talep is null) return NotFound();
        if (talep.Durum == TalepDurum.Cozuldu) return BadRequest("Çözülmüş talep tekrar atanamaz.");

        var personel = await _db.Kullanicilar.FindAsync(dto.PersonelId);
        if (personel is null || personel.RolId != 3) return BadRequest("Geçerli bir teknik personel seçiniz.");

        talep.AtananPersonelId = personel.KullaniciId;
        talep.Durum = TalepDurum.Atandi;
        OgrenciyeBildir(talep, $"Talebiniz {personel.AdSoyad} adlı personele atandı.");
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT api/arizatalepleri/5/coz
    [HttpPut("{id:int}/coz")]
    public async Task<IActionResult> Coz(int id)
    {
        var talep = await _db.ArizaTalepleri.FindAsync(id);
        if (talep is null) return NotFound();
        if (talep.Durum != TalepDurum.Atandi) return BadRequest("Sadece personele atanmış talepler çözüldü olarak işaretlenebilir.");

        talep.Durum = TalepDurum.Cozuldu;
        talep.CozumTarihi = DateTime.Now;
        OgrenciyeBildir(talep, "Arıza talebiniz çözüldü.");
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private void OgrenciyeBildir(ArizaTalebi talep, string mesaj) =>
        _db.Bildirimler.Add(new Bildirim { KullaniciId = talep.KullaniciId, Tur = "Talep", Mesaj = mesaj });
}
