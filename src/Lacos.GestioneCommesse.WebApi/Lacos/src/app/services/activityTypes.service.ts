import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { ActivityTypeModel } from '../shared/models/activity-type.model';

@Injectable()
export class ActivityTypesService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activityTypes`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readActivityTypes(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/activityTypes?${params}`)
            .pipe(
                map(e =>
                    {
                        const activityTypes: Array<ActivityTypeModel> = [];
                        e.data.forEach(item => {
                            const activityType: ActivityTypeModel = Object.assign(new ActivityTypeModel(), item);
                            activityTypes.push(activityType);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(activityTypes) : activityTypes,
                            total: e.total
                        };
                    }
                )
            );
    }

    createActivityType(request: ActivityTypeModel) {
        return this._http.post<number>(`${this._baseUrl}/activitytype`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateActivityType(request: ActivityTypeModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/activitytype/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteActivityType(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/activitytypes/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getActivityTypeDetail(id: number) {
        return this._http.get<ActivityTypeModel>(`${this._baseUrl}/activitytype-detail/${id}`)
            .pipe(
                map(e => {
                    const activityType = Object.assign(new ActivityTypeModel(), e);
                    return activityType;
                })
            );
    }

    readActivityTypesList() {
        return this._http.get<Array<ActivityTypeModel>>(`${this._baseUrl}/activitytypes-list`)
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

    readActivityTypesListCalendar() {
        return this._http.get<Array<ActivityTypeModel>>(`${this._baseUrl}/activitytypes-list-calendar`)
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
}
