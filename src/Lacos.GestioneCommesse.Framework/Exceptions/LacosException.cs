namespace Lacos.GestioneCommesse.Framework.Exceptions;

public class LacosException : Exception
{
    public LacosException()
    {

    }

    public LacosException(string message)
        : base(message)
    {

    }

    public LacosException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

    public string GetMessageRecursive()
    {
        if (InnerException is LacosException innerException)
        {
            return $"{Message} {innerException.GetMessageRecursive()}";
        }

        return Message;
    }
}