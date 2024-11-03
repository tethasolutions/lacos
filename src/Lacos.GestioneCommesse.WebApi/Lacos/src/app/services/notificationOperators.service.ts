import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { State } from '@progress/kendo-data-query';
import { ApiUrls } from './common/api-urls';
import { NotificationOperator } from '../shared/models/notificationOperator-model';
import { readData } from './common/functions';

@Injectable()
export class NotificationOperatorsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/notificationOperators`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/notificationOperators`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<NotificationOperator>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => NotificationOperator.build(e))
            );
    }

    create(notificationOperator: NotificationOperator) {
        return this._http.post<NotificationOperator>(this._baseUrl, notificationOperator)
            .pipe(
                map(e => NotificationOperator.build(e))
            );
    }

    update(notificationOperator: NotificationOperator) {
        return this._http.put<NotificationOperator>(`${this._baseUrl}/${notificationOperator.id}`, notificationOperator)
            .pipe(
                map(e => NotificationOperator.build(e))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
}
