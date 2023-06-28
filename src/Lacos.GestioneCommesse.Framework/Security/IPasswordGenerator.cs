using System.Text;

namespace Lacos.GestioneCommesse.Framework.Security;

public interface IPasswordGenerator
{
    string Generate();
}

public class PasswordGenerator : IPasswordGenerator
{
    private static readonly Random Random = new();
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private const int DefaultPasswordLength = 12;

    public string Generate()
    {
        var sb = new StringBuilder();
        var length = DefaultPasswordLength;

        while (0 < length--)
        {
            var next = Random.Next(Chars.Length);
            var @char = Chars[next];

            sb.Append(@char);
        }

        return sb.ToString();
    }
}