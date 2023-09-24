import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Ticket } from './models';
import { readData } from '../common/functions';

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
}
