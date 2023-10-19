import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Activity, ActivityDetail } from './models';
import { readData } from '../common/functions';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';

@Injectable()
export class ActivitiesService {

    static readonly asActivityDetailResolver: ResolveFn<ActivityDetail> = (r: ActivatedRouteSnapshot, _: RouterStateSnapshot) => inject(ActivitiesService).getDetail(+r.params['activityId']);

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<Activity>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => Activity.build(e))
            );
    }

    getDetail(id: number) {
        return this._http.get<ActivityDetail>(`${this._baseUrl}/${id}/detail`)
            .pipe(
                map(e => ActivityDetail.build(e))
            );
    }

    create(activity: Activity) {
        return this._http.post<Activity>(this._baseUrl, activity)
            .pipe(
                map(e => Activity.build(e))
            );
    }

    update(activity: Activity) {
        return this._http.put<Activity>(`${this._baseUrl}/${activity.id}`, activity)
            .pipe(
                map(e => Activity.build(e))
            );
    }

    save(activity: Activity) {
        return activity.id
            ? this.update(activity)
            : this.create(activity);
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }

    assignAllCustomerProducts(id: number) {
        return this._http.put<void>(`${this._baseUrl}/${id}/assign-all-customer-products`, null)
            .pipe(
                map(() => { })
            );
    }
}
