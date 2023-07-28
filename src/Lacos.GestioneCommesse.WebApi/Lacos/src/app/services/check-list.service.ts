import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { CheckListModel } from '../shared/models/check-list.model';
import { CheckListItemModel } from '../shared/models/check-list-item.model';
import { ActivityProductTypeModel } from '../shared/models/activity-product-type.model';
import { ActivityTypeModel } from '../shared/models/activity-type.model';

@Injectable()
export class CheckListService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/checklist`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readCheckList(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/checklist?${params}`)
            .pipe(
                map(e =>
                    {
                        const checkList: Array<CheckListModel> = [];
                        e.data.forEach(item => {
                            const checkListElement: CheckListModel = Object.assign(new CheckListModel(), item);
                            checkListElement.productType = Object.assign(new ActivityProductTypeModel(), checkListElement.productType);
                            checkListElement.activityType = Object.assign(new ActivityTypeModel(), checkListElement.activityType);

                            const items: Array<CheckListItemModel> = [];
                            checkListElement.items.forEach(checkListItemElement => {
                                const checkListItem: CheckListItemModel = Object.assign(new CheckListItemModel(), checkListItemElement);
                                items.push(checkListItem);
                            });
                            checkListElement.items = items;
                            checkList.push(checkListElement);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(checkList) : checkList,
                            total: e.total
                        };
                    }
                )
            );
    }

    getCheckListDetail(id: number) {
        return this._http.get<CheckListModel>(`${this._baseUrl}/checklist-detail/${id}`)
            .pipe(
                map(e => {
                    const checkListElement: CheckListModel = Object.assign(new CheckListModel(), e);
                    checkListElement.productType = Object.assign(new ActivityProductTypeModel(), checkListElement.productType);
                    checkListElement.activityType = Object.assign(new ActivityTypeModel(), checkListElement.activityType);

                    const items: Array<CheckListItemModel> = [];
                    checkListElement.items.forEach(checkListItemElement => {
                        const checkListItem: CheckListItemModel = Object.assign(new CheckListItemModel(), checkListItemElement);
                        items.push(checkListItem);
                    });
                    checkListElement.items = items;
                    return checkListElement;
                })
            );
    }

    createCheckList(request: CheckListModel) {
        const formData: FormData = new FormData();
        if (request.files.length > 0) {
            formData.append('fileKey', request.files[0], request.files[0].name);
        }
        return this._http.post<number>(`${this._baseUrl}/checklist`, formData)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateCheckList(request: CheckListModel, id: number) {
        const formData: FormData = new FormData();
        if (request.files.length > 0) {
            formData.append('fileKey', request.files[0], request.files[0].name);
        }
        return this._http.put<void>(`${this._baseUrl}/checklist/${id}`, formData)
            .pipe(
                map(() => { })
            );
    }

    deleteCheckList(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/checklist/${id}`)
            .pipe(
                map(() => { })
            );
    }

    readProductTypes() {
        return this._http.get<Array<ActivityProductTypeModel>>(`${this._baseUrl}/product-types`)
            .pipe(
                map(e =>
                    {
                        const productTypes: Array<ActivityProductTypeModel> = [];
                        e.forEach(item => {
                            const productType: ActivityProductTypeModel = Object.assign(new ActivityProductTypeModel(), item);
                            productTypes.push(productType);
                        });
                        return productTypes;
                    }
                )
            );
    }

    readActivityTypes() {
        return this._http.get<Array<ActivityTypeModel>>(`${this._baseUrl}/activity-types`)
            .pipe(
                map(e =>
                    {
                        const activityTypes: Array<ActivityTypeModel> = [];
                        e.forEach(item => {
                            const activityType: ActivityTypeModel = Object.assign(new ActivityTypeModel(), item);
                            activityTypes.push(activityType);
                        });
                        return activityTypes;
                    }
                )
            );
    }

    readCheckListItems(id: number) {
        return this._http.get<Array<CheckListItemModel>>(`${this._baseUrl}/checklist-items/${id}`)
            .pipe(
                map(e =>
                    {
                        const checkListItems: Array<CheckListItemModel> = [];
                        e.forEach(item => {
                            const checkListItem: CheckListItemModel = Object.assign(new CheckListItemModel(), item);
                            checkListItems.push(checkListItem);
                        });
                        return checkListItems;
                    }
                )
            );
    }

    getCheckListItemDetail(id: number) {
        return this._http.get<CheckListItemModel>(`${this._baseUrl}/checklist-item-detail/${id}`)
            .pipe(
                map(e => {
                    const checkListItem: CheckListItemModel = Object.assign(new CheckListItemModel(), e);
                    return checkListItem;
                })
            );
    }

    createCheckListItem(request: CheckListItemModel) {
        return this._http.post<number>(`${this._baseUrl}/checklist-item`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateCheckListItem(request: CheckListItemModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/checklist-item/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteCheckListItem(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/checklist-item/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
