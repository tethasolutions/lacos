import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map, tap } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Job, JobCopy } from './models';
import { readData } from '../common/functions';
import { JobAttachmentModel } from './job-attachment.model';
import { JobAttachmentUploadFileModel } from './job-attachment-upload-file.model';

@Injectable()
export class JobsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/jobs`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<Job>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => Job.build(e))
            );
    }

    getTicketJob(customerId: number, ticketCode: string) {
        return this._http.get<Job>(`${this._baseUrl}/getTicketJob/${customerId}/${ticketCode}`)
            .pipe(
                map(e => Job.build(e))
            );
    }

    create(job: Job) {
        return this._http.post<Job>(this._baseUrl, job)
            .pipe(
                map(e => Job.build(e))
            );
    }

    update(job: Job) {
        return this._http.put<Job>(`${this._baseUrl}/${job.id}`, job)
            .pipe(
                map(e => Job.build(e))
            );
    }
    
    copyJob(jobCopy: JobCopy) {
        return this._http.post<number>(`${this._baseUrl}/copyJob`, jobCopy);
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
    createJobAttachment(request: JobAttachmentModel) {
        return this._http.post<JobAttachmentModel>(`${this._baseUrl}/create-attachment`, request)
            .pipe(
        );
    }

    updateJobAttachment(request: JobAttachmentModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/update-attachment/${id}`, request)
            .pipe(
        );
    }

    getJobAttachments(id: number) {
        return this._http.get<Array<JobAttachmentModel>>(`${this._baseUrl}/${id}/all-attachments`)
            .pipe(
                map(e => e.map(ee => JobAttachmentModel.build(ee)))
            );
    }

    uploadJobAttachmentFile(file: File) {
        const formData = new FormData();

        formData.append(file.name, file);

        const uploadReq = new HttpRequest("POST",
            `${this._baseUrl}/job-attachment/upload-file`,
            formData,
            {
                reportProgress: false
            });

        return this._http.request(uploadReq)
            .pipe(
                filter(e => e.type === HttpEventType.Response),
                map(e => (e as HttpResponse<JobAttachmentUploadFileModel>).body),
                map(e => new JobAttachmentUploadFileModel(e.fileName, e.originalFileName))
            );
    }
    
    getJobsProgressStatus(state: State) {
        const url = `${this._baseUrl}/get-jobs-progress-status`

        return readData(this._http, state, url);
    }
}
