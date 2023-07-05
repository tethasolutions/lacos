import { JobModel } from './job.model';
import { OrderModel } from './order.model';
import { NoteAttachmentModel } from './note-attachment.model';
import { QuotationModel } from './quotation.model';
import { ActivityModel } from './activity.model';
import { CustomerModel } from './customer.model';

export class NoteModel {
    id: number;
    value: string;
    jobId: number;
    orderId: number;
    quotationId: number;
    activityId: number;
    createdOn: Date;
    operatorId: number;
    operator: CustomerModel;

    attachments: NoteAttachmentModel[];

    constructor() {
        this.id = null;
        this.value = null;

        this.jobId = null;
        this.orderId = null;
        this.quotationId = null;
        this.activityId = null;

        this.createdOn = new Date();
        this.operatorId = null;
        this.operator = null;
        this.attachments = [];
    }
}
