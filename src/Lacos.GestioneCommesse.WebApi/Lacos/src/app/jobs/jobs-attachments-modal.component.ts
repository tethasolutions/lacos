import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivityAttachmentModel } from '../services/activities/activity-attachment.model';
import { NgForm } from '@angular/forms';
import { ApiUrls } from '../services/common/api-urls';
import { JobsService } from '../services/jobs/jobs.service';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { TicketsService } from '../services/tickets/tickets.service';
import { JobAttachmentModel, JobAttachmentType } from '../services/jobs/job-attachment.model';
import { PurchaseOrderAttachmentModel } from '../services/purchase-orders/purchase-order-attachment.model';
import { TicketAttachmentModel } from '../services/tickets/ticket-attachment.model';
import { InterventionsService } from '../services/interventions/interventions.service';
import { InterventionNote } from '../services/interventions/models';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';

@Component({
    selector: 'app-jobs-attachments-modal',
    templateUrl: './jobs-attachments-modal.component.html'
})

export class JobsAttachmentsModalComponent extends ModalComponent<[number, number, number]> implements OnInit {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;
    attachments: JobAttachmentModel[];
    activityAttachments: ActivityAttachmentModel[];
    purchaseOrderAttachments: PurchaseOrderAttachmentModel[];
    purchaseOrderAdminAttachments: PurchaseOrderAttachmentModel[];
    ticketAttachments: TicketAttachmentModel[];
    interventionAttachments: InterventionNote[];
    isAdmin: boolean;

    constructor(
        private security: SecurityService,
        private readonly _jobsService: JobsService,
        private readonly _activitiesService: ActivitiesService,
        private readonly _purchaseOrdersService: PurchaseOrdersService,
        private readonly _ticketsService: TicketsService,
        private readonly _interventionService: InterventionsService,
    ) {
        super();
        this.isAdmin = security.isAuthorized(Role.Administrator);
    }

    ngOnInit() {

    }

    override open(data: [jobId: number, activityId: number, purchaseOrderId: number]) {
        const result = super.open(data);

        this._activitiesService.getActivityAttachments(data[0], data[1])
            .pipe(
                tap(e => this.activityAttachments = e)
            )
            .subscribe();

        this._interventionService.getInterventionAttachments(data[0], data[1])
            .pipe(
                tap(e => this.interventionAttachments = e)
            )
            .subscribe();

        this._jobsService.getJobAttachments(data[0])
            .pipe(
                tap(e => this.attachments = e)
            )
            .subscribe();

        this._purchaseOrdersService.getPurchaseOrderAttachments(data[0], data[2])
            .pipe(
                tap(e => this.purchaseOrderAttachments = e)
            )
            .subscribe();

        this._purchaseOrdersService.getPurchaseOrderAdminAttachments(data[0], data[2])
            .pipe(
                tap(e => this.purchaseOrderAdminAttachments = e)
            )
            .subscribe();

        this._ticketsService.getTicketAttachments(data[0])
            .pipe(
                tap(e => this.ticketAttachments = e)
            )
            .subscribe();

        return result;
    }

    protected override _canClose() {
        return true;
    }

    get visibleAttachments(): JobAttachmentModel[] {
        if (!this.attachments) return [];
        return this.isAdmin
            ? this.attachments
            : this.attachments.filter(a => a.type === JobAttachmentType.Rilievo);
    }

    public CreateUrl(fileName: string, displayName: string): string {
        return `${this._baseUrl}/activity-attachment/download-file/${fileName}/${displayName}`;
    }

    public attachmentTypeName(type: JobAttachmentType): string {
        switch (type) {
            case JobAttachmentType.Preventivo: return 'Preventivo';
            case JobAttachmentType.Rilievo: return 'Rilievo';
            default: return 'Altro';
        }
    }
}
