import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { InterventionProduct } from './models';
import { readData } from '../common/functions';

@Injectable()
export class InterventionProductsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/intervention-products`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    create(interventionProduct: InterventionProduct) {
        return this._http.post<void>(this._baseUrl, interventionProduct)
            .pipe(
                map(() => { })
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
