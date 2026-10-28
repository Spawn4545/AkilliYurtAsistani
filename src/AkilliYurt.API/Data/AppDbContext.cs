using AkilliYurt.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AkilliYurt.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rol> Roller => Set<Rol>();
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Bina> Binalar => Set<Bina>();
    public DbSet<Oda> Odalar => Set<Oda>();
    public DbSet<ArizaTalebi> ArizaTalepleri => Set<ArizaTalebi>();
    public DbSet<Duyuru> Duyurular => Set<Duyuru>();
    public DbSet<Bildirim> Bildirimler => Set<Bildirim>();
    public DbSet<CamasirMakinesi> CamasirMakineleri => Set<CamasirMakinesi>();
    public DbSet<CamasirRezervasyon> CamasirRezervasyonlari => Set<CamasirRezervasyon>();
    public DbSet<IotCihaz> IotCihazlar => Set<IotCihaz>();
    public DbSet<SensorOlcum> SensorOlcumleri => Set<SensorOlcum>();
    public DbSet<GirisCikis> GirisCikislar => Set<GirisCikis>();
    public DbSet<YemekMenusu> YemekMenuleri => Set<YemekMenusu>();
    public DbSet<YemekDegerlendirme> YemekDegerlendirmeleri => Set<YemekDegerlendirme>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Birincil anahtarlar (isimleri EF konvansiyonuna uymayanlar)
        mb.Entity<ArizaTalebi>().HasKey(x => x.TalepId);
        mb.Entity<CamasirMakinesi>().HasKey(x => x.MakineId);
        mb.Entity<CamasirRezervasyon>().HasKey(x => x.RezervasyonId);
        mb.Entity<IotCihaz>().HasKey(x => x.CihazId);
        mb.Entity<SensorOlcum>().HasKey(x => x.OlcumId);
        mb.Entity<GirisCikis>().HasKey(x => x.KayitId);
        mb.Entity<YemekMenusu>().HasKey(x => x.MenuId);
        mb.Entity<YemekDegerlendirme>().HasKey(x => x.DegerlendirmeId);

        // Benzersiz alanlar
        mb.Entity<Kullanici>().HasIndex(x => x.Eposta).IsUnique();
        mb.Entity<Kullanici>().HasIndex(x => x.RfidKartNo).IsUnique().HasFilter("[RfidKartNo] IS NOT NULL");
        mb.Entity<IotCihaz>().HasIndex(x => x.MacAdresi).IsUnique();

        // İlişkiler
        mb.Entity<Kullanici>().HasOne(x => x.Rol).WithMany(r => r.Kullanicilar).HasForeignKey(x => x.RolId);
        mb.Entity<Kullanici>().HasOne(x => x.Oda).WithMany(o => o.Ogrenciler).HasForeignKey(x => x.OdaId);
        mb.Entity<Oda>().HasOne(x => x.Bina).WithMany(b => b.Odalar).HasForeignKey(x => x.BinaId);

        mb.Entity<ArizaTalebi>().HasOne(x => x.Kullanici).WithMany(k => k.ArizaTalepleri).HasForeignKey(x => x.KullaniciId);
        mb.Entity<ArizaTalebi>().HasOne(x => x.AtananPersonel).WithMany().HasForeignKey(x => x.AtananPersonelId);
        mb.Entity<ArizaTalebi>().HasOne(x => x.Oda).WithMany().HasForeignKey(x => x.OdaId);

        mb.Entity<Duyuru>().HasOne(x => x.Yayinlayan).WithMany().HasForeignKey(x => x.YayinlayanId);
        mb.Entity<Bildirim>().HasOne(x => x.Kullanici).WithMany(k => k.Bildirimler).HasForeignKey(x => x.KullaniciId);

        mb.Entity<CamasirMakinesi>().HasOne(x => x.Bina).WithMany(b => b.CamasirMakineleri).HasForeignKey(x => x.BinaId);
        mb.Entity<CamasirRezervasyon>().HasOne(x => x.Makine).WithMany(m => m.Rezervasyonlar).HasForeignKey(x => x.MakineId);
        mb.Entity<CamasirRezervasyon>().HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId);

        mb.Entity<IotCihaz>().HasOne(x => x.Oda).WithMany(o => o.Cihazlar).HasForeignKey(x => x.OdaId);
        mb.Entity<SensorOlcum>().HasOne(x => x.Cihaz).WithMany(c => c.Olcumler).HasForeignKey(x => x.CihazId);
        mb.Entity<GirisCikis>().HasOne(x => x.Cihaz).WithMany().HasForeignKey(x => x.CihazId);
        mb.Entity<GirisCikis>().HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId);

        mb.Entity<YemekDegerlendirme>().HasOne(x => x.Menu).WithMany(m => m.Degerlendirmeler).HasForeignKey(x => x.MenuId);
        mb.Entity<YemekDegerlendirme>().HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId);

        // Enum'lar veritabanında okunabilir metin olarak saklansın
        mb.Entity<ArizaTalebi>().Property(x => x.Durum).HasConversion<string>().HasMaxLength(20);
        mb.Entity<CamasirMakinesi>().Property(x => x.Durum).HasConversion<string>().HasMaxLength(20);
        mb.Entity<GirisCikis>().Property(x => x.Yon).HasConversion<string>().HasMaxLength(10);

        // SQL Server'da çoklu cascade yolu hatasını önlemek için silmeleri kısıtla
        foreach (var fk in mb.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // Başlangıç verileri
        mb.Entity<Rol>().HasData(
            new Rol { RolId = 1, RolAdi = "Ogrenci" },
            new Rol { RolId = 2, RolAdi = "Yonetici" },
            new Rol { RolId = 3, RolAdi = "TeknikPersonel" });
    }
}
