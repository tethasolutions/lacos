using System.Security.Cryptography;
using System.Text;

namespace Lacos.GestioneCommesse.Framework.Security;

public interface IPasswordHasher
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
}

public class PasswordHasher : IPasswordHasher
{
    public string GenerateSalt()
    {
        var bytes = new byte[32];

        using (var keyGenerator = RandomNumberGenerator.Create())
        {
            keyGenerator.GetBytes(bytes);

            var result = BitConverter.ToString(bytes)
                .Replace("-", "")
                .ToLower();

            return result;
        }
    }

    public string HashPassword(string password, string salt)
    {
        var saltedPassword = password + salt;

        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.Unicode.GetBytes(saltedPassword);
            var hashedBytes = sha256.ComputeHash(bytes);
            var result = BitConverter.ToString(hashedBytes)
                .Replace("-", "")
                .ToLower();

            return result;
        }
    }
}