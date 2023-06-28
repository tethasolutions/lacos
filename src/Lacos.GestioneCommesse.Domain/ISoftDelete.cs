namespace Lacos.GestioneCommesse.Domain;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
}