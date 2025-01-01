import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { Ticket, TicketStatus } from '../services/tickets/models';
import { filter, map, switchMap, tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { getToday, listEnum } from '../services/common/functions';
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
import { MessageModalComponent } from '../messages/message-modal.component';
import { MessageModalOptions, MessageModel, MessageReadModel } from '../services/messages/models';
import { User } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { UserService } from '../services/security/user.service';
import { MessagesService } from '../services/messages/messages.service';
import { PurchaseOrder, PurchaseOrderStatus } from '../services/purchase-orders/models';
import { PurchaseOrderModalComponent, PurchaseOrderModalOptions } from '../purchase-order/purchase-order-modal.component';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { GalleryModalComponent, GalleryModalInput } from '../shared/gallery-modal.component';

@Component({
    selector: 'app-ticket-modal',
    templateUrl: 'ticket-modal.component.html'
})
export class TicketModalComponent extends ModalFormComponent<Ticket> implements OnInit {

    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('purchaseOrderModal', { static: true }) purchaseOrderModal: PurchaseOrderModalComponent;
    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    @ViewChild('galleryModal', { static: true }) galleryModal: GalleryModalComponent;

    customers: CustomerModel[];
    _job: Job;
    attachments: Array<FileInfo> = [];
    messages: MessageReadModel[];
    user: User;
    currentOperator: OperatorModel;
    unreadMessages: number;
    album: string[] = [];
    targetOperatorsArray: number[];
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/tickets`;
    pathImage = `${ApiUrls.baseAttachmentsUrl}/`;
    uploadSaveUrl = `${this._baseUrl}/ticket-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/ticket-attachment/remove-file`;
    
    readonly states = listEnum<TicketStatus>(TicketStatus);

    constructor(
        private readonly _customersService: CustomerService,
        private readonly _serviceJob: JobsService,
        private readonly _serviceActivity: ActivitiesService,
        private readonly _servicePurchaseOrder: PurchaseOrdersService,
        messageBox: MessageBoxService,
        private readonly _operatorsService: OperatorsService,
        private readonly _user: UserService,
        private readonly _messagesService: MessagesService
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._getData();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
    }

    override open(ticket: Ticket) {
        const result = super.open(ticket);
        
        this.attachments = [];
        this.album = [];
        if (ticket.pictures != null) {
            this.options.pictures.forEach(element => {
                if (element.description != null && element.fileName != null) {
                    this.attachments.push({ name: element.description });
                    this.album.push(this.pathImage + element.fileName);
                }
            });
        }

        this.updateUnreadCounter();
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
                    tap(() => {
                        this.options.jobId = this._job.id;
                        this._newActivity(ticket); 
                    })
                )
                .subscribe()
        );

    }

    private _newActivity(ticket: Ticket) {
        const activity = new Activity(0, ActivityStatus.Pending, null, null, `Rif. Ticket: ${ticket.code}<br/>${ticket.description}`, null,
            this._job.id, null, null, null, null, null, null, "In attesa", "In corso", "Pronto", "Completata", [], []);
        const options = new ActivityModalOptions(activity);

        ticket.status = TicketStatus.InProgress;

        this._subscriptions.push(
            this.activityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._serviceActivity.create(activity)),
                    tap(e => ticket.activityId = e.id),
                    tap(e => ticket.jobId = this._job.id),
                    tap(e => this._messageBox.success(`Ticket aggiornato con successo`)),
                    tap(() => this.close())
                )
                .subscribe()
        );
    }

    createPurchaseOrder(ticket: Ticket) {

        this._subscriptions.push(
            this._serviceJob.getTicketJob(ticket.customerId, ticket.code.replace("/","-"))
                .pipe(
                    tap(e => this._job = e),
                    tap (e => this.options.jobId = this._job.id),
                    tap(() => this._newPurchaseOrder(ticket))
                )
                .subscribe()
        );

    }

    private _newPurchaseOrder(ticket: Ticket) {
        const today = getToday();
        const purchaseOrder = new PurchaseOrder(0, null, today.getFullYear(), today, null, `Rif. Ticket: ${ticket.code}<br/>${ticket.description}`, PurchaseOrderStatus.Pending, 
            this._job.id, null, null, this.currentOperator.id, [], [], [], [], []);
        const options = new PurchaseOrderModalOptions(purchaseOrder);

        ticket.status = TicketStatus.InProgress;

        this._subscriptions.push(
            this.purchaseOrderModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._servicePurchaseOrder.create(purchaseOrder)),
                    tap(e => ticket.jobId = this._job.id),
                    tap(e => ticket.purchaseOrderId = e.id),
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
    
    protected _getCurrentOperator(userId: number) {
        this._subscriptions.push(
            this._operatorsService.getOperatorByUserId(userId)
                .pipe(
                    tap(e => this.currentOperator = e)
                )
                .subscribe()
        );
    }

    initNewMessage() {
        this.targetOperatorsArray = [];
        if (this.options.id == 0) 
        {
            this._messageBox.info("Prima di creare il nuovo commento è necessario salvare l'elemento corrente");
            return;
        }
        this.createMessage();
        // this._subscriptions.push(
        //     this._messagesService.getElementTargetOperators(this.currentOperator.id, this.options.id, "T")
        //         .pipe(
        //             tap(e => {
        //                 this.targetOperatorsArray = e;
        //                 this.createMessage();
        //             })
        //         )
        //         .subscribe()
        // );
    }

    createMessage() {
        const today = new Date();
        const message = new MessageModel(0, today, null, this.currentOperator.id, null, null, this.options.id, null, false);
        const options = new MessageModalOptions(message, true, true, this.targetOperatorsArray);

        this._subscriptions.push(
            this.messageModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._messagesService.create(message, options.targetOperators.join(","))),
                    tap(e => {
                        var msg = new MessageReadModel(e.id, e.date, e.note, e.operatorId, this.currentOperator.name, e.jobId, e.activityId, e.ticketId, e.purchaseOrderId, "", true, false, null);
                        this.options.messages.push(msg);
                    }),
                    tap(() => this._messageBox.success('Commento creato'))
                )
                .subscribe()
        );
    }

    markAsRead(message: MessageReadModel) {
        this._subscriptions.push(
            this._messagesService.markAsRead(message.id, this.currentOperator.id)
                .pipe(
                    tap(() => {
                        message.isRead = true;
                        this._messageBox.success('Commento letto');
                        this.updateUnreadCounter();
                    })
                )
                .subscribe()
        );
    }

    private _afterMessageUpdated(message: MessageModel) {
        this._messageBox.success('Commento aggiornato.');
        const originalMsg = this.options.messages.find(e => e.id == message.id);
        originalMsg.date = message.date;
        originalMsg.note = message.note;
        this.updateUnreadCounter();
        //this._read();
    }
    
    editMessage(message: MessageReadModel) {
        this._subscriptions.push(
            this._messagesService.get(message.id)
                .pipe(
                    map(e => new MessageModalOptions(e,false)),
                    switchMap(e => this.messageModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._messagesService.update(this.messageModal.options.message)),
                    tap(() => this._afterMessageUpdated(this.messageModal.options.message))
                )
                .subscribe()
        );
    }

    deleteMessage(message: MessageReadModel) {
        this._messageBox.confirm(`Sei sicuro di voler cancellare il commento?`, 'Conferma l\'azione').subscribe(result => {
            if (result == true) {
                this._subscriptions.push(
                    this._messagesService.delete(message.id)
                        .pipe(
                            tap(e => {
                                this.options.messages.remove(message);
                                this.updateUnreadCounter();
                            }),
                            tap(e => this._messageBox.success(`Commento cancellato con successo`))
                        )
                        .subscribe()
                );
            }
        });
    }
    
    updateUnreadCounter() {
        this.unreadMessages = this.options.messages.count(e => !e.isRead);
    }
    
    openImage(index: number) {
        const options = new GalleryModalInput(this.album, index);
        this.galleryModal.open(options).subscribe();
    }

}
