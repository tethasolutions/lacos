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
import { IJobReadModel, Job } from '../services/jobs/models';
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
import { State } from '@progress/kendo-data-query';
import { AddressesService } from '../services/addresses.service';
import { AddressModel } from '../shared/models/address.model';
import { AddressModalComponent } from '../address-modal/address-modal.component';

@Component({
    selector: 'app-ticket-modal',
    templateUrl: 'ticket-modal.component.html'
})
export class TicketModalComponent extends ModalFormComponent<Ticket> implements OnInit {

    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('purchaseOrderModal', { static: true }) purchaseOrderModal: PurchaseOrderModalComponent;
    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    @ViewChild('galleryModal', { static: true }) galleryModal: GalleryModalComponent;

    customers: CustomerModel[];
    attachments: Array<FileInfo> = [];
    messages: MessageReadModel[];
    user: User;
    currentOperator: OperatorModel;
    unreadMessages: number;
    album: string[] = [];
    targetOperatorsArray: number[];
    jobs: SelectableJob[];
    addresses: AddressModel[];

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
        private readonly _messagesService: MessagesService,
        private readonly _jobsService: JobsService,
        private readonly _addressService: AddressesService
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

        this._getJobs();
        this.updateUnreadCounter();
        return result;
    }

    onDateChange() {
        this.options.year = this.options.date?.getFullYear();
    }

    onCustomerChange() {
        this._getJobs();
        this._readAddresses(this.options.customerId);
    }

    protected _readAddresses(customerId: number) {
        this._subscriptions.push(
            this._addressService.getCustomerAddresses(customerId)
                .pipe(
                    map(e => {
                        this.addresses = e;
                        if (this.options.addressId) {
                            const address = this.addresses.find(a => a.id === this.options.addressId);
                            if (address) {
                                this.options.addressId = address.id;
                            } else {
                                this.options.addressId = null;
                            }
                        }
                    }),
                    tap(() => { })
                )
                .subscribe()
        );
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

        if (this.options.customerId) {
            state.filter.filters.push(
                { field: 'customerId', operator: 'eq', value: this.options.customerId }
            );
            this._readAddresses(this.options.customerId);
        }

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e))),
                )
                .subscribe()
        );
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

    createJobTicket() {
        const text = `Vuoi creare una nuova commessa per il ticket corrente?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Nuova commessa')
                .pipe(
                    tap(() => {
                        this._subscriptions.push(
                            this._serviceJob.getTicketJob(this.options.customerId, this.options.addressId, this.options.code.replace("/", "-"), this.options.description)
                                .pipe(
                                    tap(e => this.options.jobId = e.id),
                                    tap(() => {
                                        this._getJobs();
                                    })
                                )
                                .subscribe()
                        );
                    })
                )
                .subscribe()
        );
    }

    createActivity(ticket: Ticket) {
        if (this.options.jobId) {
            this._newActivity(ticket);
        }
        else {
            this._messageBox.info("Prima di creare l'attività è necessario creare una commessa");
        }
    }

    createAddress() {
        const request = new AddressModel();
        this._subscriptions.push(
            this.addressModal.open(request)
                .pipe(
                    filter(e => e),
                    tap(() => {
                        this.addNewAddress(request);
                    })
                )
                .subscribe()
        );
    }

    addNewAddress(address: AddressModel) {
        if (this.options.customerId !== null) address.customerId = this.options.customerId;
        this._subscriptions.push(
            this._addressService.createAddress(address)
                .pipe(
                    tap(e => {
                        this._readAddresses(this.options.customerId);
                        this.options.addressId = e.id;
                        this._messageBox.success(`Indirizzo creato con successo`);
                    })
                )
                .subscribe()
        );
    }

    private _newActivity(ticket: Ticket) {
        const activity = new Activity(0, ActivityStatus.Pending, null, null, `Rif. Ticket: ${ticket.code}<br/>${ticket.description}`, null,
            ticket.jobId, null, ticket.addressId, null, null, null, null, false, "In attesa", "In corso", "Pronto", "Completata", false, false, false, false, [], []);
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

    createPurchaseOrder(ticket: Ticket) {
        if (this.options.jobId) {
            this._newPurchaseOrder(ticket);
        }
        else {
            this._messageBox.info("Prima di creare l'attività è necessario creare una commessa");
        }
    }

    private _newPurchaseOrder(ticket: Ticket) {
        const today = getToday();
        const purchaseOrder = new PurchaseOrder(0, null, today.getFullYear(), today, null, `Rif. Ticket: ${ticket.code}<br/>${ticket.description}`, PurchaseOrderStatus.Pending, null,
            null, null, this.currentOperator.id, [ticket.jobId], [], [], [], [], [], []);
        const options = new PurchaseOrderModalOptions(purchaseOrder);

        ticket.status = TicketStatus.InProgress;

        this._subscriptions.push(
            this.purchaseOrderModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._servicePurchaseOrder.create(purchaseOrder)),
                    tap(e => ticket.purchaseOrderId = e.id),
                    tap(e => this._messageBox.success(`Ticket aggiornato con successo`)),
                    tap(() => this.close())
                )
                .subscribe()
        );
    }

    override close() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
            return;
        }

        if (this.options.id != 0 && this.options.activityId == null && this.options.purchaseOrderId == null) {
            this._subscriptions.push(
                this._messageBox.confirm(`Non è stata creata un\'attività o un ordine di acquisto collegato al ticket, chiudere comunque?`, `Conferma chiusura`)
                    .pipe(
                        filter(result => result),
                        tap(() => super.close())
                    )
                    .subscribe()
            );
        } else {
            super.close();
        }
    }

    protected override _canClose() {
        return true;
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
        if (this.options.id == 0) {
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
                        var msg = new MessageReadModel(e.id, e.date, e.note, e.operatorId, this.currentOperator.name, e.jobId, e.activityId, e.ticketId, e.purchaseOrderId, "", true, [], false, null);
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
                    map(e => new MessageModalOptions(e, false)),
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
