import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { readData } from '../common/functions';
import { HelperDocumentModel } from './models';

@Injectable()
export class HelperDocumentsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/helperDocuments`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<HelperDocumentModel>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => {return Object.assign(new HelperDocumentModel(), e)})
            );
    }

    create(helperDocument: HelperDocumentModel) {
        return this._http.post<HelperDocumentModel>(this._baseUrl, helperDocument)
            .pipe(
                map(e => {return Object.assign(new HelperDocumentModel(), e)})
            );
    }

    update(helperDocument: HelperDocumentModel) {
        return this._http.put<HelperDocumentModel>(`${this._baseUrl}/${helperDocument.id}`, helperDocument)
            .pipe(
                map(e => {return Object.assign(new HelperDocumentModel(), e)})
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
}
