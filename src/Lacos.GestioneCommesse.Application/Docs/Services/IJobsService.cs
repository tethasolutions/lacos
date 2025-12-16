using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IJobsService
{
    IQueryable<JobReadModel> Query();
    IQueryable<JobSelectableModel> GetSelectables();
    IQueryable<JobReadModel> QueryCurrentJobs();
    IQueryable<JobReadModel> QueryArchivedJobs();
    Task<JobDto> GetTicketJob(TicketJobRequest request);
    Task<JobDto> Get(long id, bool showInterventionNotes = false);
    Task<JobDto> Create(JobDto jobDto);
    Task<JobDto> Update(JobDto jobDto);
    Task Delete(long id);
    Task<long> CopyJob(JobCopyDto jobCopyDto);
    Task<IEnumerable<JobAttachmentReadModel>> GetJobAttachments(long jobId);
    Task<JobAttachmentReadModel> GetJobAttachmentDetail(long attachmentId);
    Task<JobAttachmentReadModel> DownloadJobAttachment(string filename);
    Task<JobAttachmentDto> UpdateJobAttachment(long id, JobAttachmentDto attachmentDto);
    Task<JobAttachmentDto> CreateJobAttachment(JobAttachmentDto attachmentDto);
    IQueryable<JobsProgressStatusReadModel> GetJobsProgressStatus();
}