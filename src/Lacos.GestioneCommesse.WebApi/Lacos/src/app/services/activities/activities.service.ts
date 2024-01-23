import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Activity, ActivityCounter, ActivityDetail, ActivityStatus } from './models';
import { readData } from '../common/functions';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { ActivityAttachmentModel } from './activity-attachment.model';
import { ActivityAttachmentUploadFileModel } from './activity-attachment-upload-file.model';

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

    readActivityTypesCounters() {
        return this._http.get<Array<ActivityCounter>>(`${this._baseUrl}/activities-counters`)
            .pipe(
                map(e => {
                    const activityTypes: Array<ActivityCounter> = [];
                    e.forEach(item => {
                        const activityType: ActivityCounter = Object.assign(new ActivityCounter(), item);
                        activityTypes.push(activityType);
                    });
                    return activityTypes;
                }
                )
            );
    }

    createActivityAttachment(request: ActivityAttachmentModel) {
        return this._http.post<ActivityAttachmentModel>(`${this._baseUrl}/create-attachment`, request)
            .pipe(
        );
    }

    updateActivityAttachment(request: ActivityAttachmentModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/update-attachment/${id}`, request)
            .pipe(
        );
    }

    getActivityAttachments(id: number) {
        return this._http.get<Array<ActivityAttachmentModel>>(`${this._baseUrl}/${id}/all-attachments`)
            .pipe(
                map(e => e.map(ee => ActivityAttachmentModel.build(ee)))
            );
    }

    getActivityAttachmentDetail(id: number) {
        return this._http.get<ActivityDetail>(`${this._baseUrl}/attachment-detail/${id}`)
            .pipe(
                map(e => ActivityDetail.build(e))
            );
    }

    uploadActivityAttachmentFile(file: File) {
        const formData = new FormData();

        formData.append(file.name, file);

        const uploadReq = new HttpRequest("POST",
            `${this._baseUrl}/activity-attachment/upload-file`,
            formData,
            {
                reportProgress: false
            });

        return this._http.request(uploadReq)
            .pipe(
                filter(e => e.type === HttpEventType.Response),
                map(e => (e as HttpResponse<ActivityAttachmentUploadFileModel>).body),
                map(e => new ActivityAttachmentUploadFileModel(e.fileName, e.originalFileName))
            );
    }

    deleteActivity(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/activity/${id}`)
            .pipe(
                map(() => { })
            );
    }
}
