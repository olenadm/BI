using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BIMagistral.Helpers;

public class Criptography
{
    public record Password
    {
        public string Pwd { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }

    public static Password HashPassword(string rawPassword, string? salt = null)
    {
        byte[] saltBytes =
            (salt == null ? RandomNumberGenerator.GetBytes(128 / 8) : Convert.FromBase64String(salt));
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: rawPassword,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return new Password() { Pwd = hashed, Salt = Convert.ToBase64String(saltBytes) };
    }

    public static bool CheckPassword(string hash, string salt, string raw)
    {
        return hash.Equals(HashPassword(raw, salt).Pwd);
    }
}