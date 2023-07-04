import { JobModel } from './job.model';
import { ContactModel } from './contact.model';
import { NoteModel } from './note.model';
import { OrderStatusEnum } from '../enums/order-status.enum';

export class OrderModel {
    code: string;
    description: string;
    expirationDate: Date;
    status: OrderStatusEnum;
    statusChangedOn: Date;
    jobId: number;
    job: JobModel;
    supplierId: number;
    supplier: ContactModel;
    notes: NoteModel[];
}