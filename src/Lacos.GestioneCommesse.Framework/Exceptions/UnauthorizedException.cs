namespace Lacos.GestioneCommesse.Framework.Exceptions;

public class UnauthorizedException : LacosException
{
    public UnauthorizedException()
    {

    }

    public UnauthorizedException(string message)
        : base(message)
    {

    }
}