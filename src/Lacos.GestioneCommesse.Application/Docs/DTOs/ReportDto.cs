using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ReportDto
{
    public Byte[] Content { get; set; }
    public string FileName { get; set; }

    public ReportDto(Byte[] content, string fileName)
    {
        Content = content;
        FileName = fileName;
    }
}