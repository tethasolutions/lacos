import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Ticket, TicketCounter } from './models';
import { readData } from '../common/functions';
import { TicketAttachmentModel } from './ticket-attachment.model';
import { TicketAttachmentUploadFileModel } from './ticket-attachment-upload-file.model';

@Injectable()
export class TicketsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/tickets`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<Ticket>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => Ticket.build(e))
            );
    }

    create(ticket: Ticket) {
        return this._http.post<Ticket>(this._baseUrl, ticket)
            .pipe(
                map(e => Ticket.build(e))
            );
    }

    update(ticket: Ticket) {
        return this._http.put<Ticket>(`${this._baseUrl}/${ticket.id}`, ticket)
            .pipe(
                map(e => Ticket.build(e))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
    readTicketsCounters() {
        return this._http.get<TicketCounter>(`${this._baseUrl}/tickets-counters`)
            .pipe(
                map(e => TicketCounter.build(e))
            );
    }
    
    createTicketAttachment(request: TicketAttachmentModel) {
        return this._http.post<TicketAttachmentModel>(`${this._baseUrl}/create-attachment`, request)
            .pipe(
        );
    }

    updateTicketAttachment(request: TicketAttachmentModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/update-attachment/${id}`, request)
            .pipe(
        );
    }

    getTicketAttachments(id: number) {
        return this._http.get<Array<TicketAttachmentModel>>(`${this._baseUrl}/${id}/all-attachments`)
            .pipe(
                map(e => e.map(ee => TicketAttachmentModel.build(ee)))
            );
    }

    uploadTicketAttachmentFile(file: File) {
        const formData = new FormData();

        formData.append(file.name, file);

        const uploadReq = new HttpRequest("POST",
            `${this._baseUrl}/ticket-attachment/upload-file`,
            formData,
            {
                reportProgress: false
            });

        return this._http.request(uploadReq)
            .pipe(
                filter(e => e.type === HttpEventType.Response),
                map(e => (e as HttpResponse<TicketAttachmentUploadFileModel>).body),
                map(e => new TicketAttachmentUploadFileModel(e.fileName, e.originalFileName))
            );
    }
}
