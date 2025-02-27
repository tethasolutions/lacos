import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map, tap } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { readData } from '../common/functions';
import { MessageModel, MessageReadModel, MessagesListReadModel } from './models';
import { UserService } from '../security/user.service';

@Injectable()
export class MessagesService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/messages`;

    constructor(
        private readonly _http: HttpClient,
        private readonly _userService: UserService
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
                map(e => e.map(ee => MessageReadModel.build(ee, this._userService.getUser().operatorId)))
            );
    }
    
    getMessagesList(state: State, operatorId: number) {
        const url = `${this._baseUrl}/${operatorId}/get-messageslist`

        return readData(this._http, state, url);
    }

    create(message: MessageModel, targetOperators: string) {
        return this._http.put<MessageModel>(`${this._baseUrl}/create/${targetOperators}`, message)
            .pipe(
                map(e => MessageModel.build(e))
            );
    }

    createReply(message: MessageModel, targetOperators: string) {
        return this._http.put<MessageModel>(`${this._baseUrl}/create-reply/${targetOperators}`, message)
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
    
    getReplyTargetOperators(messageId: number, replyAll: boolean) {
        return this._http.get<Array<number>>(`${this._baseUrl}/${messageId}/${replyAll}/get-target-operators`)
            .pipe(
                map(e => e)
            );
    }

    getMessageTargetOperators(messageId: number) {
        return this._http.get<Array<number>>(`${this._baseUrl}/${messageId}/get-message-target-operators`)
            .pipe(
                map(e => e)
            );
    }

    getElementTargetOperators(senderOperatorId: number, elementId: number, elementType: string) {
        return this._http.get<Array<number>>(`${this._baseUrl}/${senderOperatorId}/${elementId}/${elementType}/get-target-operators-by-element`)
            .pipe(
                map(e => e)
            );
    }

    getUnreadCounter(operatorId: number) {
        return this._http.get<number>(`${this._baseUrl}/unread-counter/${operatorId}`)
            .pipe(
                map(e => e)
            );
    }

    getUnreadCounterFromApp(operatorId: number) {
        return this._http.get<number>(`${this._baseUrl}/unread-counter-fromApp/${operatorId}`)
            .pipe(
                map(e => e)
            );
    }
}
