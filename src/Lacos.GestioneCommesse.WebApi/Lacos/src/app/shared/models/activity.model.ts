import { JobModel } from './job.model';
import { NoteModel } from './note.model';
import { UserModel } from './user.model';
import { ActivityStatusEnum } from '../enums/activity-status.enum';

export class ActivityModel {
    id: number;
    description: string;
    start: Date;
    end: Date;
    status: ActivityStatusEnum;
    statusChangedOn: Date;
    operatorId: number;
    operator: UserModel;
    jobId: number;
    job: JobModel;
    notes: NoteModel[];

    constructor() {
        this.id = null;
        this.description = null;
        this.start = new Date();
        this.end = new Date((new Date()).getTime() + (1000 * 60 * 60 * 24 * 2));
        this.status = ActivityStatusEnum.Planned;
        this.statusChangedOn = new Date();
        this.operatorId = null;
        this.operator = new UserModel();
        this.jobId = null;
        this.job = new JobModel();
        this.notes = [];
    }
}
