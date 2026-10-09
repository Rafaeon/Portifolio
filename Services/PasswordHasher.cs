using System.Security.Cryptography;

namespace Portifolio.Services;

public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public static (string Salt, string Hash) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool Verify(string password, string saltBase64, string hashBase64)
    {
        var salt = Convert.FromBase64String(saltBase64);
        var expected = Convert.FromBase64String(hashBase64);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
