import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { PurchaseOrder, PurchaseOrderItem } from './models';
import { readData } from '../common/functions';

@Injectable()
export class PurchaseOrdersService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/purchase-orders`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    getItem(id: number) {
        return this._http.get<PurchaseOrderItem>(`${this._baseUrl}/purchase-order-item/${id}`)
            .pipe(
                map(e => PurchaseOrderItem.build(e))
            );
    }

    get(id: number) {
        return this._http.get<PurchaseOrder>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => PurchaseOrder.build(e))
            );
    }

    create(purchaseOrder: PurchaseOrder) {
        return this._http.post<PurchaseOrder>(this._baseUrl, purchaseOrder)
            .pipe(
                map(e => PurchaseOrder.build(e))
            );
    }

    update(purchaseOrder: PurchaseOrder) {
        return this._http.put<PurchaseOrder>(`${this._baseUrl}/${purchaseOrder.id}`, purchaseOrder)
            .pipe(
                map(e => PurchaseOrder.build(e))
            );
    }

    updateItem(item: PurchaseOrderItem) {
        return this._http.put<PurchaseOrderItem>(`${this._baseUrl}/purchase-order-item/${item.id}`, item)
            .pipe(
                map(e => PurchaseOrderItem.build(e))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
}
