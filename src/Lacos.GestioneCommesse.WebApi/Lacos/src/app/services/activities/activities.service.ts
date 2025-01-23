import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { Activity, ActivityCounter, ActivityDetail, ActivityStatus, CopyActivityModel, NewActivityCounter } from './models';
import { readData } from '../common/functions';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { ActivityAttachmentModel } from './activity-attachment.model';
import { ActivityAttachmentUploadFileModel } from './activity-attachment-upload-file.model';
import { UserService } from '../security/user.service';

@Injectable()
export class ActivitiesService {

    static readonly asActivityDetailResolver: ResolveFn<ActivityDetail> = (r: ActivatedRouteSnapshot, _: RouterStateSnapshot) => inject(ActivitiesService).getDetail(+r.params['activityId']);

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;

    constructor(
        private readonly _http: HttpClient,
        private readonly _userService: UserService
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    activitiesFromProduct(state: State, product: string) {
        const url = `${this._baseUrl}/${product}/activitiesFromProduct`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<Activity>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => Activity.build(e, this._userService.getUser().operatorId))
            );
    }

    getDetail(id: number) {
        return this._http.get<ActivityDetail>(`${this._baseUrl}/${id}/detail`)
            .pipe(
                map(e => ActivityDetail.build(e, this._userService.getUser().operatorId))
            );
    }

    create(activity: Activity) {
        return this._http.post<Activity>(this._baseUrl, activity)
            .pipe(
                map(e => Activity.build(e, this._userService.getUser().operatorId))
            );
    }

    update(activity: Activity) {
        return this._http.put<Activity>(`${this._baseUrl}/${activity.id}`, activity)
            .pipe(
                map(e => Activity.build(e, this._userService.getUser().operatorId))
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

    assignAllCustomerProductsMonthlyMaint(id: number) {
        return this._http.put<void>(`${this._baseUrl}/${id}/assign-all-customer-products-monthlymaint`, null)
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

    readNewActivitiesCounter() {
        return this._http.get<NewActivityCounter>(`${this._baseUrl}/new-activities-counter`)
            .pipe(
                map(e => NewActivityCounter.build(e))
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

    getActivityAttachments(jobId: number, activityId: number) {
        return this._http.get<Array<ActivityAttachmentModel>>(`${this._baseUrl}/all-attachments/${jobId}/${activityId}`)
            .pipe(
                map(e => e.map(ee => ActivityAttachmentModel.build(ee)))
            );
    }

    getActivityAttachmentDetail(id: number) {
        return this._http.get<ActivityDetail>(`${this._baseUrl}/attachment-detail/${id}`)
            .pipe(
                map(e => ActivityDetail.build(e, this._userService.getUser().operatorId))
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
    
    copyActivityToJob(copyActivityModel: CopyActivityModel) {
        return this._http.post<CopyActivityModel>(`${this._baseUrl}/copy-activity`, copyActivityModel)
            .pipe(
        );
    }
}
