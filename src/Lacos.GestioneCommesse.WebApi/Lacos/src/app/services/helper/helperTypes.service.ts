import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { readData } from '../common/functions';
import { HelperTypeModel } from './models';

@Injectable()
export class HelperTypesService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/helperTypes`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<HelperTypeModel>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => HelperTypeModel.build(e))
            );
    }

    create(helperType: HelperTypeModel) {
        return this._http.post<HelperTypeModel>(this._baseUrl, helperType)
            .pipe(
                map(e => HelperTypeModel.build(e))
            );
    }

    update(helperType: HelperTypeModel) {
        return this._http.put<HelperTypeModel>(`${this._baseUrl}/${helperType.id}`, helperType)
            .pipe(
                map(e => HelperTypeModel.build(e))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
}
