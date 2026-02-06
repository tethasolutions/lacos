import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { WarehouseMovementModel } from '../shared/models/warehouse-movement.model';

@Injectable()
export class WarehouseService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/warehouse`;

    constructor(
        private readonly _http: HttpClient
    ) { }

    readMovements(productId: number, state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/movements/${productId}?${params}`)
            .pipe(
                map(e =>
                    <GridDataResult>{
                        data: hasGroups ? translateDataSourceResultGroups(e.data) : e.data,
                        total: e.total
                    }
                )
            );
    }

    getMovement(id: number) {
        return this._http.get<WarehouseMovementModel>(`${this._baseUrl}/movement/${id}`)
            .pipe(
                map(e => Object.assign(new WarehouseMovementModel(), e))
            );
    }

    createMovement(request: WarehouseMovementModel) {
        return this._http.post<WarehouseMovementModel>(`${this._baseUrl}/movement`, request)
            .pipe(
                map(e => Object.assign(new WarehouseMovementModel(), e))
            );
    }

    updateMovement(request: WarehouseMovementModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/movement/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteMovement(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/movement/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
