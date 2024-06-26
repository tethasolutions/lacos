import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map, tap } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { readData } from '../common/functions';
import { MessageModel, MessageReadModel } from './models';

@Injectable()
export class MessagesService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/messages`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<MessageModel>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => MessageModel.build(e))
            );
    }

    getMessages(jobId: number, activityId: number, ticketId: number, purchaseOrderId: number) {
        return this._http.get<Array<MessageReadModel>>(`${this._baseUrl}/get-messages/${jobId}/${activityId}/${ticketId}/${purchaseOrderId}`)
            .pipe(
                map(e => e.map(ee => MessageReadModel.build(ee)))
            );
    }

    create(message: MessageModel) {
        return this._http.post<MessageModel>(this._baseUrl, message)
            .pipe(
                map(e => MessageModel.build(e))
            );
    }

    update(message: MessageModel) {
        return this._http.put<MessageModel>(`${this._baseUrl}/${message.id}`, message)
            .pipe(
                map(e => MessageModel.build(e))
            );
    }
    
    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }

    markAsRead(messageId: number, operatorId: number) {
        return this._http.get(`${this._baseUrl}/mark-as-read/${messageId}/${operatorId}`)
            .pipe(
                map(() => { })
            );
    }
    
}
