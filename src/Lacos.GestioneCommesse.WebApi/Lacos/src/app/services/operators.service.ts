import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { OperatorModel } from '../shared/models/operator.model';
import { VehicleModel } from '../shared/models/vehicle.model';
import { OperatorDocumentModel } from '../shared/models/operator-document.model';

@Injectable()
export class OperatorsService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/operators`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readOperators(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/operators?${params}`)
            .pipe(
                map(e =>
                    {
                        const operators: Array<OperatorModel> = [];
                        e.data.forEach(item => {
                            const operator: OperatorModel = Object.assign(new OperatorModel(), item);
                            operator.defaultVehicle = Object.assign(new VehicleModel(), operator.defaultVehicle);

                            const documents: Array<OperatorDocumentModel> = [];
                            operator.documents.forEach(docItem => {
                                const document: OperatorDocumentModel = Object.assign(new OperatorDocumentModel(), docItem);
                                documents.push(document);
                            });
                            operator.documents = documents;

                            operators.push(operator);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(operators) : operators,
                            total: e.total
                        };
                    }
                )
            );
    }

    createOperator(request: OperatorModel) {
        return this._http.post<number>(`${this._baseUrl}/operator`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateOperator(request: OperatorModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/operator/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteOperator(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/operator/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getOperatorDetail(id: number) {
        return this._http.get<OperatorModel>(`${this._baseUrl}/operator-detail/${id}`)
            .pipe(
                map(e => {
                    const operator = Object.assign(new OperatorModel(), e);
                    operator.defaultVehicle = Object.assign(new VehicleModel(), operator.defaultVehicle);
                    const documents: Array<OperatorDocumentModel> = [];
                    operator.documents.forEach(docItem => {
                        const document: OperatorDocumentModel = Object.assign(new OperatorDocumentModel(), docItem);
                        documents.push(document);
                    });
                    operator.documents = documents;
                    return operator;
                })
            );
    }

    getOperatorDocument(id: number) {
        return this._http.get<OperatorDocumentModel>(`${this._baseUrl}/operator-document/${id}`)
            .pipe(
                map(e => {
                    const document = Object.assign(new OperatorDocumentModel(), e);
                    return document;
                })
            );
    }

    createDocument(request: OperatorDocumentModel) {
        return this._http.post<number>(`${this._baseUrl}/document`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateDocument(request: OperatorDocumentModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/document/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteDocument(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/document/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
