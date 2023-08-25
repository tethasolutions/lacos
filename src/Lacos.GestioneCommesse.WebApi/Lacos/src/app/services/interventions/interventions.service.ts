import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Intervention } from './models';
import { readData } from '../common/functions';

@Injectable()
export class InterventionsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/interventions`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<Intervention>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => Intervention.build(e))
            );
    }

    create(intervention: Intervention) {
        return this._http.post<Intervention>(this._baseUrl, intervention)
            .pipe(
                map(e => Intervention.build(e))
            );
    }

    update(intervention: Intervention) {
        return this._http.put<Intervention>(`${this._baseUrl}/${intervention.id}`, intervention)
            .pipe(
                map(e => Intervention.build(e))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
