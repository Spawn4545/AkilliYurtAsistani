using System.ComponentModel.DataAnnotations;

namespace AkilliYurt.API.Dtos;

// ESP32'nin gönderdiği JSON: { "macAdresi": "AA:BB:..", "sicaklik": 24.5, "nem": 40, "gazSeviyesi": 120 }
public record OlcumGonderDto(
    [Required] string MacAdresi,
    double Sicaklik,
    double Nem,
    int GazSeviyesi);

public record OlcumCevapDto(bool Alarm, string Mesaj);

// RFID okuyucunun gönderdiği JSON: { "macAdresi": "AA:BB:..", "kartNo": "04A1B2C3" }
public record KartOkutDto([Required] string MacAdresi, [Required] string KartNo);
