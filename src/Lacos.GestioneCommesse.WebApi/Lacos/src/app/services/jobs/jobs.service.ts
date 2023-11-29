import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Job } from './models';
import { readData } from '../common/functions';

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

    getTicketJob(customerId: number) {
        return this._http.get<Job>(`${this._baseUrl}/getTicketJob/${customerId}`)
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

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
