# Akıllı Yurt Asistanı

IoT destekli yurt yönetim sistemi – **Mesleki Uygulama ve Proje** dersi dönem projesi.

| | |
|---|---|
| **Öğrenci** | Furkan Koyuncu (256001029) |
| **Danışman** | Erkan Güler |

## Projenin Amacı

Yurtlardaki günlük işlerin (arıza bildirimi, duyurular, yemek menüsü, çamaşırhane, giriş-çıkış takibi) tek bir mobil uygulama üzerinden dijital olarak yönetilmesi; ESP32 ile odalardaki sıcaklık, nem ve gaz seviyesinin izlenmesi ve RFID kart ile giriş-çıkışların otomatik kaydedilmesi.

## Kullanılan Teknolojiler

- **Backend:** C#, ASP.NET Core 8 Web API
- **Veritabanı:** Microsoft SQL Server, Entity Framework Core
- **Mobil:** .NET MAUI
- **Donanım:** ESP32, DHT22, MQ-2, RC522 RFID

## Modüller

1. Kullanıcı ve yetki yönetimi
2. Oda ve öğrenci yönetimi
3. Arıza / talep yönetimi
4. Duyuru ve bildirim
5. Yemekhane menüsü
6. Çamaşırhane rezervasyonu
7. IoT ortam izleme (ESP32)
8. RFID giriş-çıkış takibi (ESP32)
9. Raporlama ve analiz

## Klasör Yapısı

```
AkilliYurtAsistani/
├── docs/                    # Proje bildirim formu ve diyagramlar
│   └── diyagramlar/
└── src/
    └── AkilliYurt.API/      # ASP.NET Core Web API
        ├── Controllers/
        ├── Data/
        ├── Dtos/
        └── Models/
```

## Diyagramlar

- [ER Diyagramı](docs/diyagramlar/er-diyagrami.png)
- [Use Case – Arıza/Talep Yönetimi](docs/diyagramlar/usecase-ariza-talep.png)
- [Use Case – IoT Ortam İzleme ve Giriş-Çıkış](docs/diyagramlar/usecase-iot-izleme.png)
