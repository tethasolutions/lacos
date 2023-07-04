import { JobModel } from './job.model';
import { NoteModel } from './note.model';
import { QuotationStatusEnum } from '../enums/quotation-status.enum';

export class QuotationModel {
    amount: number;
    expirationDate: Date;
    status: QuotationStatusEnum;
    statusChangedOn: Date;
    jobId: number;
    job: JobModel;
    notes: NoteModel[];
}
