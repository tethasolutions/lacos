namespace Lacos.GestioneCommesse.Framework.Exceptions;

public class NotFoundException : LacosException
{
    public NotFoundException(Type entityType, long id)
        : base($"Entity of type [{entityType.Name}] with id [{id}] not found.")
    {

    }

    public NotFoundException(string message)
        : base(message)
    {

    }
}