using System.Security.Cryptography;

namespace Lacos.GestioneCommesse.Framework.Security;

public interface IAccessTokenGenerator
{
    string Generate();
}

public class AccessTokenGenerator : IAccessTokenGenerator
{
    public string Generate()
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
}