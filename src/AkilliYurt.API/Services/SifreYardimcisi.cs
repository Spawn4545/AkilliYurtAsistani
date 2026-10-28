using System.Security.Cryptography;

namespace AkilliYurt.API.Services;

// Şifreler düz metin olarak değil, PBKDF2 (SHA-256) ile hash'lenip tuz (salt) ile saklanır.
public static class SifreYardimcisi
{
    private const int TuzBoyutu = 16;
    private const int HashBoyutu = 32;
    private const int Tekrar = 100_000;

    public static string Hashle(string sifre)
    {
        byte[] tuz = RandomNumberGenerator.GetBytes(TuzBoyutu);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(sifre, tuz, Tekrar, HashAlgorithmName.SHA256, HashBoyutu);
        return $"{Convert.ToBase64String(tuz)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Dogrula(string sifre, string kayitliHash)
    {
        var parcalar = kayitliHash.Split('.');
        if (parcalar.Length != 2) return false;

        byte[] tuz = Convert.FromBase64String(parcalar[0]);
        byte[] beklenen = Convert.FromBase64String(parcalar[1]);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(sifre, tuz, Tekrar, HashAlgorithmName.SHA256, HashBoyutu);
        return CryptographicOperations.FixedTimeEquals(hash, beklenen);
    }
}
