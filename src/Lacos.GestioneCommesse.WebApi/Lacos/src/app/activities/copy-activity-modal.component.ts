import { Component, OnInit, ViewChild } from '@angular/core';
import { Activity, ActivityStatus, CopyActivityModel } from '../services/activities/models';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { filter, map, switchMap, tap } from 'rxjs';
import { ActivityTypesService } from '../services/activityTypes.service';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModel } from '../shared/models/customer.model';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { IJobReadModel, Job } from '../services/jobs/models';
import { getToday, listEnum } from '../services/common/functions';
import { AddressModel } from '../shared/models/address.model';
import { AddressesService } from '../services/addresses.service';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { ActivityAttachmentUploadFileModel } from '../services/activities/activity-attachment-upload-file.model';
import { FileInfo, SuccessEvent } from '@progress/kendo-angular-upload';
import { SupplierModel } from '../shared/models/supplier.model';
import { SupplierService } from '../services/supplier.service';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { ActivityAttachmentModel } from '../services/activities/activity-attachment.model';
import { SupplierModalComponent } from '../supplier-modal/supplier-modal.component';
import { MessageModalOptions, MessageModel, MessageReadModel } from '../services/messages/models';
import { User } from '../services/security/models';
import { UserService } from '../services/security/user.service';
import { MessagesService } from '../services/messages/messages.service';
import { MessageModalComponent } from '../messages/message-modal.component';
import { GalleryModalComponent, GalleryModalInput } from '../shared/gallery-modal.component';

@Component({
    selector: 'app-copy-activity-modal',
    templateUrl: 'copy-activity-modal.component.html'
})
export class CopyActivityModalComponent extends ModalFormComponent<CopyActivityModel> implements OnInit {

    jobs: SelectableJob[];

    constructor(
        messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._getJobs();
    }

    override open(options: CopyActivityModel) {
        const result = super.open(options);

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getJobs() {
        const state: State = {
            filter: {
                filters: [],
                logic: 'and'
            },
            sort: [
                { field: 'date', dir: 'asc' }
            ]
        };

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e))),
                )
                .subscribe()
        );
    }
}

class SelectableJob {

    readonly id: number;
    readonly customer: string;
    readonly code: string;
    readonly fullName: string;
    readonly customerId: number;
    readonly addressId: number;
    readonly description: string;

    constructor(
        job: IJobReadModel
    ) {
        this.id = job.id;
        this.customer = job.customer;
        this.code = job.code;
        this.fullName = `${job.code} - ${job.customer}` + ((job.reference) ? ` - ${job.reference}` : ``);
        this.customerId = job.customerId;
        this.addressId = job.addressId;
        this.description = job.description;
    }

}