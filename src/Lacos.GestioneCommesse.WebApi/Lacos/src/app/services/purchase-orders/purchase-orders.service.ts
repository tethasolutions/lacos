import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State } from '@progress/kendo-data-query';
import { PurchaseOrder, PurchaseOrderItem } from './models';
import { readData } from '../common/functions';
import { PurchaseOrderAttachmentModel } from './purchase-order-attachment.model';
import { PurchaseOrderAttachmentUploadFileModel } from './purchage-order-attachment-upload-file.model';
import { UserService } from '../security/user.service';

@Injectable()
export class PurchaseOrdersService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/purchase-orders`;

    constructor(
        private readonly _http: HttpClient,
        private readonly _userService: UserService
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<PurchaseOrder>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => PurchaseOrder.build(e, this._userService.getUser().operatorId))
            );
    }

    create(purchaseOrder: PurchaseOrder) {
        return this._http.post<PurchaseOrder>(this._baseUrl, purchaseOrder)
            .pipe(
                map(e => PurchaseOrder.build(e, this._userService.getUser().operatorId))
            );
    }

    update(purchaseOrder: PurchaseOrder) {
        return this._http.put<PurchaseOrder>(`${this._baseUrl}/${purchaseOrder.id}`, purchaseOrder)
            .pipe(
                map(e => PurchaseOrder.build(e, this._userService.getUser().operatorId))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }

    createPurchaseOrderAttachment(request: PurchaseOrderAttachmentModel) {
        return this._http.post<PurchaseOrderAttachmentModel>(`${this._baseUrl}/create-attachment`, request)
            .pipe(
        );
    }

    updatePurchaseOrderAttachment(request: PurchaseOrderAttachmentModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/update-attachment/${id}`, request)
            .pipe(
        );
    }

    getPurchaseOrderAttachments(jobId: number, purchaseOrderId: number) {
        return this._http.get<Array<PurchaseOrderAttachmentModel>>(`${this._baseUrl}/all-attachments/${jobId}/${purchaseOrderId}`)
            .pipe(
                map(e => e.map(ee => PurchaseOrderAttachmentModel.build(ee)))
            );
    }

    getPurchaseOrderAdminAttachments(jobId: number, purchaseOrderId: number) {
        return this._http.get<Array<PurchaseOrderAttachmentModel>>(`${this._baseUrl}/all-admin-attachments/${jobId}/${purchaseOrderId}`)
            .pipe(
                map(e => e.map(ee => PurchaseOrderAttachmentModel.build(ee)))
            );
    }

    uploadPurchaseOrderAttachmentFile(file: File) {
        const formData = new FormData();

        formData.append(file.name, file);

        const uploadReq = new HttpRequest("POST",
            `${this._baseUrl}/job-attachment/upload-file`,
            formData,
            {
                reportProgress: false
            });

        return this._http.request(uploadReq)
            .pipe(
                filter(e => e.type === HttpEventType.Response),
                map(e => (e as HttpResponse<PurchaseOrderAttachmentUploadFileModel>).body),
                map(e => new PurchaseOrderAttachmentUploadFileModel(e.fileName, e.originalFileName))
            );
    }
}
