import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivityAttachmentModel } from '../services/activities/activity-attachment.model';
import { NgForm } from '@angular/forms';
import { ApiUrls } from '../services/common/api-urls';
import { JobsService } from '../services/jobs/jobs.service';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { TicketsService } from '../services/tickets/tickets.service';
import { JobAttachmentModel } from '../services/jobs/job-attachment.model';
import { PurchaseOrderAttachmentModel } from '../services/purchase-orders/purchase-order-attachment.model';
import { TicketAttachmentModel } from '../services/tickets/ticket-attachment.model';

@Component({
    selector: 'app-jobs-attachments-modal',
    templateUrl: './jobs-attachments-modal.component.html'
})

export class JobsAttachmentsModalComponent extends ModalComponent<number> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;
    attachments: JobAttachmentModel[];
    activityAttachments: ActivityAttachmentModel[];
    purchaseOrderAttachments: PurchaseOrderAttachmentModel[];
    ticketAttachments: TicketAttachmentModel[];

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
        private readonly _activitiesService: ActivitiesService,
        private readonly _purchaseOrdersService: PurchaseOrdersService,
        private readonly _ticketsService: TicketsService
    ) {
        super();
    }

    ngOnInit() {

    }

    override open(jobId: number) {
        const result = super.open(jobId);

        this._jobsService.getJobAttachments(jobId)
            .pipe(
                tap(e => this.attachments = e)
            )
            .subscribe();

        this._activitiesService.getActivityAttachments(jobId)
            .pipe(
                tap(e => this.activityAttachments = e)
            )
            .subscribe();

        this._purchaseOrdersService.getPurchaseOrderAttachments(jobId)
            .pipe(
                tap(e => this.purchaseOrderAttachments = e)
            )
            .subscribe();

        this._ticketsService.getTicketAttachments(jobId)
            .pipe(
                tap(e => this.ticketAttachments = e)
            )
            .subscribe();

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    public CreateUrl(fileName: string, displayName: string): string {
        return `${this._baseUrl}/activity-attachment/download-file/${fileName}/${displayName}`;
    }
}
