import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { Ticket, TicketStatus } from '../services/tickets/models';
import { filter, switchMap, tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { listEnum } from '../services/common/functions';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { Activity, ActivityStatus } from '../services/activities/models';
import { ActivityModalComponent, ActivityModalOptions } from '../activities/activity-modal.component';
import { ActivitiesService } from '../services/activities/activities.service';
import { JobsService } from '../services/jobs/jobs.service';
import { Job } from '../services/jobs/models';
import { TicketAttachmentModel } from '../services/tickets/ticket-attachment.model';
import { FileInfo, SuccessEvent } from '@progress/kendo-angular-upload';
import { TicketAttachmentUploadFileModel } from '../services/tickets/ticket-attachment-upload-file.model';
import { ApiUrls } from '../services/common/api-urls';

@Component({
    selector: 'app-ticket-modal',
    templateUrl: 'ticket-modal.component.html'
})
export class TicketModalComponent extends ModalComponent<Ticket> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;

    customers: CustomerModel[];
    _job: Job;
    attachments: Array<FileInfo> = [];
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/tickets`;
    uploadSaveUrl = `${this._baseUrl}/ticket-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/ticket-attachment/remove-file`;
    
    readonly states = listEnum<TicketStatus>(TicketStatus);

    constructor(
        private readonly _customersService: CustomerService,
        private readonly _serviceJob: JobsService,
        private readonly _serviceActivity: ActivitiesService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._getData();
    }

    override open(ticket: Ticket) {
        const result = super.open(ticket);
        
        this.attachments = [];
        if (ticket.pictures != null) {
            this.options.pictures.forEach(element => {
                if (element.description != null && element.fileName != null) {
                    this.attachments.push({ name: element.description });
                }
            });
        }

        return result;
    }

    onDateChange() {
        this.options.year = this.options.date?.getFullYear();
    }

    createCustomer() {
        const request = new CustomerModel();
        request.fiscalType = 1;

        this._subscriptions.push(
            this.customerModal.open(request)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._customersService.createCustomer(request)),
                    tap(e => {
                        this.options.customerId = e.id;
                        this._messageBox.success(`Cliente ${request.name} creato`);
                    }),
                    tap(() => this._getData())
                )
                .subscribe()
        );
    }

    createActivity(ticket: Ticket) {

        this._subscriptions.push(
            this._serviceJob.getTicketJob(ticket.customerId, ticket.code.replace("/","-"))
                .pipe(
                    tap(e => this._job = e),
                    tap(() => this._newActivity(ticket))
                )
                .subscribe()
        );

    }

    private _newActivity(ticket: Ticket) {
        const activity = new Activity(0, ActivityStatus.Pending, null, null, null, `Rif. Ticket: ${ticket.code}<br/>${ticket.description}`,
            this._job.id, null, null, null, null, null, null, "In attesa", "In corso", "Completata", []);
        const options = new ActivityModalOptions(activity);

        ticket.status = TicketStatus.InProgress;

        this._subscriptions.push(
            this.activityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._serviceActivity.create(activity)),
                    tap(e => ticket.activityId = e.id),
                    tap(e => this._messageBox.success(`Ticket aggiornato con successo`)),
                    tap(() => this.close())
                )
                .subscribe()
        );
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getData() {
        this._subscriptions.push(
            this._customersService.getCustomersList()
                .pipe(
                    tap(e => this._setData(e))
                )
                .subscribe()
        );
    }

    private _setData(customers: CustomerModel[]) {
        this.customers = customers;
    }

    downloadAttachment(fileName: string) {
        const attachment = this.options.pictures
            .find(e => e.description === fileName);
        const url = `${this._baseUrl}/ticket-attachment/download-file/${attachment.fileName}/${attachment.description}`;

        window.open(url);
    }

    public AttachmentExecutionSuccess(e: SuccessEvent): void {
        const file = e.response.body as TicketAttachmentUploadFileModel;
        if (file != null) {
            let ticketAttachmentModal = new TicketAttachmentModel(0, file.originalFileName, file.fileName, this.options.id);
            this.options.pictures.push(ticketAttachmentModal);
        } else {
            const deletedFile = e.files[0].name;
            this.options.pictures.findAndRemove(e => e.description === deletedFile);
        }
    }
}
