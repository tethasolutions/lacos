import { Component, OnInit, ViewChild } from '@angular/core';
import { Activity, ActivityStatus } from '../services/activities/models';
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
    selector: 'app-activity-modal',
    templateUrl: 'activity-modal.component.html'
})
export class ActivityModalComponent extends ModalFormComponent<ActivityModalOptions> implements OnInit {

    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
    @ViewChild('supplierModal', { static: true }) supplierModal: SupplierModalComponent;
    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    @ViewChild('galleryModal', { static: true }) galleryModal: GalleryModalComponent;

    activityTypes: ActivityTypeModel[];
    customer: CustomerModel;
    jobs: SelectableJob[];
    job: Job;
    jobReadonly: boolean;
    status: ActivityStatus;
    selectedActivityType: ActivityTypeModel;
    selectedJob: SelectableJob;
    suppliers: SupplierModel[];
    addresses: AddressModel[];
    operators: OperatorModel[];
    messages: MessageReadModel[];
    user: User;
    currentOperator: OperatorModel;
    unreadMessages: number;

    attachments: Array<FileInfo> = [];
    album: string[] = [];
    targetOperatorsArray: number[];

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;

    pathImage = `${ApiUrls.baseAttachmentsUrl}/`;
    uploadSaveUrl = `${this._baseUrl}/activity-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/activity-attachment/remove-file`;

    readonly states = listEnum<ActivityStatus>(ActivityStatus);

    constructor(
        private readonly _activityTypesService: ActivityTypesService,
        messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
        private readonly _suppliersService: SupplierService,
        private readonly _addressesService: AddressesService,
        private readonly _operatorsService: OperatorsService,
        private readonly _user: UserService,
        private readonly _messagesService: MessagesService
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._getActivityTypes();
        this._getSuppliers();
        this._getOperators();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
    }

    onJobChanged() {
        this.options.activity.addressId = null;

        this._tryGetAddress();
    }

    onActivityTypeChange() {
        this.selectedActivityType = this.activityTypes.find(e => e.id == this.options.activity.typeId);
        this.options.activity.shortDescription = this.selectedActivityType.name;
        this.selectedJob = this.jobs.find(e => e.id == this.options.activity.jobId);
        if (this.selectedActivityType.isInternal) {
            this.onSupplierChange();
            if (!this.options.activity.supplierId) {
                this.options.activity.addressId = null;
                const customerId = this.jobs
                    .find(e => e.id === this.options.activity.jobId).customerId;
                this.readAddresses(customerId);
            }
            //this.options.activity.description = this.selectedJob.description;
        }
        else {
            const customerId = this.jobs
                .find(e => e.id === this.options.activity.jobId).customerId;
            this.readAddresses(customerId);
        }
    }

    createSupplier() {
        const request = new SupplierModel();

        this._subscriptions.push(
            this.supplierModal.open(request)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._suppliersService.createSupplier(request)),
                    tap(e => {
                        this.options.activity.supplierId = e.id;
                        this._messageBox.success(`Fornitore ${request.name} creato`);
                    }),
                    tap(() => this._getSuppliers())
                )
                .subscribe()
        );
    }

    onSupplierChange() {
        const selectedSupplier = this.suppliers.find(e => e.id == this.options.activity.supplierId);
        this.addresses = selectedSupplier?.addresses ?? [];
        if (selectedSupplier != undefined) {
            const selectedAddress: AddressModel = selectedSupplier.addresses.find(x => x.isMainAddress == true);
            if (selectedAddress != undefined) {
                this.options.activity.addressId = selectedAddress.id;
            }
        }
    }

    override open(options: ActivityModalOptions) {
        const result = super.open(options);

        this.attachments = [];
        this.album = [];
        if (options.activity.attachments != null) {
            this.options.activity.attachments.forEach(element => {
                if (element.displayName != null && element.fileName != null) {
                    this.attachments.push({ name: element.displayName });
                    if (element.isImage) this.album.push(this.pathImage + element.fileName);
                    if (!element.isImage) this.album.push("assets/document.jpg");
                }
            });
        }
        if (this.options.activity.jobId) {
            this._subscriptions.push(
                this._jobsService.get(this.options.activity.jobId)
                    .pipe(
                        tap(e => {
                            this.job = e;
                        })
                    )
                    .subscribe()
            );
        }

        this.jobReadonly = !!options.activity.jobId;
        this.customer = null;
        this.status = options.activity.status;
        this._getJobs();

        if (this.options.activity.typeId) {
            this.selectedActivityType = this.activityTypes.find(e => e.id == this.options.activity.typeId);
            if (this.selectedActivityType.isInternal && this.options.activity.supplierId) {
                this.onSupplierChange();
            }
        }

        this.updateUnreadCounter();
        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getActivityTypes() {
        this._subscriptions.push(
            this._activityTypesService.readActivityTypesList()
                .pipe(
                    tap(e => this.activityTypes = e)
                )
                .subscribe()
        );
    }

    private _getSuppliers() {
        this._subscriptions.push(
            this._suppliersService.getSuppliersList()
                .pipe(
                    tap(e => this._setData(e)),
                    tap(() => {
                        if (this.selectedActivityType) {
                            if (this.selectedActivityType.isInternal && this.options.activity.supplierId) {
                                this.onSupplierChange();
                            }
                        }
                    })

                )
                .subscribe()
        );
    }

    private _getOperators() {
        const state: State = {
            sort: [
                { field: 'name', dir: 'asc' }
            ]
        };

        this._subscriptions.push(
            this._operatorsService.readOperators(state)
                .pipe(
                    tap(e => this.operators = e.data as OperatorModel[])
                )
                .subscribe()
        )
    }

    private _setData(suppliers: SupplierModel[]) {
        this.suppliers = suppliers;
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

        if (this.options.activity.jobId) {
            state.filter.filters.push(
                { field: 'id', operator: 'eq', value: this.options.activity.jobId }
            );
        }

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e))),
                    tap(() => this._tryGetAddress())
                )
                .subscribe()
        );
    }

    private _tryGetAddress() {
        if (!this.options.activity.jobId) {
            this.addresses = [];
            this.options.activity.addressId = null;
            return;
        }

        const customerId = this.jobs
            .find(e => e.id === this.options.activity.jobId).customerId;

        this.readAddresses(customerId);

        if (this.options.activity.addressId) return;

        const addressId = this.jobs
            .find(e => e.id === this.options.activity.jobId).addressId;
        this.options.activity.addressId = addressId;
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
        if (this.job.customerId !== null) address.customerId = this.job.customerId;
        this._subscriptions.push(
            this._addressesService.createAddress(address)
                .pipe(
                    map(e => e),
                    tap(e => {
                        this.readAddresses(this.job.customerId);
                        this.options.activity.addressId = e.id;
                        this._messageBox.success(`Indirizzo creato con successo`);
                    })
                )
                .subscribe()
        );
    }

    readAddresses(customerId: number) {
        this._subscriptions.push(
            this._addressesService.getCustomerAddresses(customerId)
                .pipe(
                    map(e => {
                        this.addresses = e;
                    }),
                    tap(() => { })
                )
                .subscribe()
        );
    }

    downloadAttachment(fileName: string) {
        const attachment = this.options.activity.attachments
            .find(e => e.displayName === fileName);
        const url = `${this._baseUrl}/activity-attachment/download-file/${attachment.fileName}/${attachment.displayName}`;

        window.open(url);
    }

    public AttachmentExecutionSuccess(e: SuccessEvent): void {
        const file = e.response.body as ActivityAttachmentUploadFileModel;
        if (file != null) {
            let activityAttachmentModal = new ActivityAttachmentModel(0, file.originalFileName, file.fileName, this.options.activity.id);
            this.options.activity.attachments.push(activityAttachmentModal);
        } else {
            const deletedFile = e.files[0].name;
            this.options.activity.attachments.findAndRemove(e => e.displayName === deletedFile);
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
        if (this.options.activity.id == 0) 
        {
            this._messageBox.info("Prima di creare il nuovo commento è necessario salvare l'elemento corrente");
            return;
        }
        this.createMessage();
        // this._subscriptions.push(
        //     this._messagesService.getElementTargetOperators(this.currentOperator.id, this.options.activity.id, "A")
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
        const message = new MessageModel(0, today, null, this.currentOperator.id, null, this.options.activity.id, null, null);
        const options = new MessageModalOptions(message,true,true, this.targetOperatorsArray);

        this._subscriptions.push(
            this.messageModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._messagesService.create(message, options.targetOperators.join(","))),
                    tap(e => {
                        var msg = new MessageReadModel(e.id, e.date, e.note, e.operatorId, this.currentOperator.name, e.jobId, e.activityId, e.ticketId, e.purchaseOrderId, "", true);
                        this.options.activity.messages.push(msg);
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
        const originalMsg = this.options.activity.messages.find(e => e.id == message.id);
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
                                this.options.activity.messages.remove(message);
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
        this.unreadMessages = this.options.activity.messages.count(e => !e.isRead);
    }

    openImage(index: number) {
        const options = new GalleryModalInput(this.album, index);
        this.galleryModal.open(options).subscribe();
    }

}

export class ActivityModalOptions {

    constructor(
        readonly activity: Activity
    ) {
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
