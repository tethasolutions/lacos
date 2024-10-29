using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface ITicketsService
{
    IQueryable<TicketReadModel> Query();
    Task<TicketDto> Get(long id);
    Task<TicketDto> Create(TicketDto jobDto);
    Task<TicketDto> Update(TicketDto jobDto);
    Task Delete(long id);
    Task<int> GetNextNumber(int year);
    Task<TicketCounterDto> GetTicketsCounters();

    Task<IEnumerable<TicketAttachmentReadModel>> GetTicketAttachments(long jobId);
    Task<TicketAttachmentReadModel> GetTicketAttachmentDetail(long attachmentId);
    Task<TicketAttachmentReadModel> DownloadTicketAttachment(string filename);
    Task<TicketAttachmentDto> UpdateTicketAttachment(long id, TicketAttachmentDto attachmentDto);
    Task<TicketAttachmentDto> CreateTicketAttachment(TicketAttachmentDto attachmentDto);
    Task<ReportDto> GenerateReport(long ticketId);
}