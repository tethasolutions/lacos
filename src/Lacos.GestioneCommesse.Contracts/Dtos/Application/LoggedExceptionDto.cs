namespace Lacos.GestioneCommesse.Contracts.Dtos.Application;

public class LoggedExceptionDto
{
    public string Title { get; set; }
    public string Exception { get; set; }
    public string Device { get; set; }
    public string LacosSession { get; set; }
    public DateTime Date { get; set; }
}