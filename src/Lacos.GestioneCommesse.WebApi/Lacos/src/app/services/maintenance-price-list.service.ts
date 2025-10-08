import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { MaintenancePriceListItemModel, MaintenancePriceListModel } from '../shared/models/maintenance-price-list.model';

@Injectable()
export class MaintenancePriceListService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/maintenancePriceLists`;

    constructor(
        private readonly _http: HttpClient
    ) { }

    readMaintenancePriceList(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/maintenancePriceList?${params}`)
            .pipe(
                map(e => {
                    const maintenancePriceList: Array<MaintenancePriceListModel> = [];
                    e.data.forEach(item => {
                        const maintenancePriceListElement: MaintenancePriceListModel = Object.assign(new MaintenancePriceListModel(), item);

                        const items: Array<MaintenancePriceListItemModel> = [];
                        maintenancePriceListElement.items.forEach(item => {
                            const maintenancePriceListItem: MaintenancePriceListItemModel = Object.assign(new MaintenancePriceListItemModel(), item);
                            items.push(maintenancePriceListItem);
                        });
                        maintenancePriceListElement.items = items;
                        maintenancePriceList.push(maintenancePriceListElement);
                    });
                    return <GridDataResult>{
                        data: hasGroups ? translateDataSourceResultGroups(maintenancePriceList) : maintenancePriceList,
                        total: e.total
                    };
                }
                )
            );
    }

    getMaintenancePriceListDetail(id: number) {
        return this._http.get<MaintenancePriceListModel>(`${this._baseUrl}/maintenancePriceList-detail/${id}`)
            .pipe(
                map(e => {
                    const maintenancePriceListElement: MaintenancePriceListModel = Object.assign(new MaintenancePriceListModel(), e);

                    const items: Array<MaintenancePriceListItemModel> = [];
                    maintenancePriceListElement.items.forEach(item => {
                        const maintenancePriceListItem: MaintenancePriceListItemModel = Object.assign(new MaintenancePriceListItemModel(), item);
                        items.push(maintenancePriceListItem);
                    });
                    maintenancePriceListElement.items = items;
                    return maintenancePriceListElement;
                })
            );
    }

    createMaintenancePriceList(request: MaintenancePriceListModel) {
        return this._http.post<number>(`${this._baseUrl}/maintenancePriceList`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateMaintenancePriceList(request: MaintenancePriceListModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/maintenancePriceList/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteMaintenancePriceList(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/maintenancePriceList/${id}`)
            .pipe(
                map(() => { })
            );
    }

    readMaintenancePriceListItems(id: number) {
        return this._http.get<Array<MaintenancePriceListItemModel>>(`${this._baseUrl}/maintenancePriceList-items/${id}`)
            .pipe(
                map(e => {
                    const maintenancePriceListItems: Array<MaintenancePriceListItemModel> = [];
                    e.forEach(item => {
                        const maintenancePriceListItem: MaintenancePriceListItemModel = Object.assign(new MaintenancePriceListItemModel(), item);
                        maintenancePriceListItems.push(maintenancePriceListItem);
                    });
                    return maintenancePriceListItems;
                }
                )
            );
    }

    getMaintenancePriceListItemDetail(id: number) {
        return this._http.get<MaintenancePriceListItemModel>(`${this._baseUrl}/maintenancePriceList-item-detail/${id}`)
            .pipe(
                map(e => {
                    const maintenancePriceListItem: MaintenancePriceListItemModel = Object.assign(new MaintenancePriceListItemModel(), e);
                    return maintenancePriceListItem;
                })
            );
    }

    createMaintenancePriceListItem(request: MaintenancePriceListItemModel) {
        return this._http.post<number>(`${this._baseUrl}/maintenancePriceList-item`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateMaintenancePriceListItem(request: MaintenancePriceListItemModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/maintenancePriceList-item/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteMaintenancePriceListItem(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/maintenancePriceList-item/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
