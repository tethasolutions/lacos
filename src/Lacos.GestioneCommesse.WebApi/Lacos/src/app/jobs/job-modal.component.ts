import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { Job, JobStatus } from '../services/jobs/models';
import { filter, map, switchMap, tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { refreshUserData, SecurityService } from '../services/security/security.service';
import { AddressesService } from '../services/addresses.service';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { AddressModel } from '../shared/models/address.model';
import { WindowState } from '@progress/kendo-angular-dialog';
import { JobsService } from '../services/jobs/jobs.service';
import { listEnum } from '../services/common/functions';
import { ApiUrls } from '../services/common/api-urls';
import { FileInfo, SuccessEvent } from '@progress/kendo-angular-upload';
import { JobAttachmentUploadFileModel } from '../services/jobs/job-attachment-upload-file.model';
import { JobAttachmentModel, JobAttachmentType } from '../services/jobs/job-attachment.model';
import { State } from '@progress/kendo-data-query';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { MessageModalOptions, MessageModel, MessageReadModel } from '../services/messages/models';
import { MessagesService } from '../services/messages/messages.service';
import { Role, User } from '../services/security/models';
import { UserService } from '../services/security/user.service';
import { MessageModalComponent } from '../messages/message-modal.component';
import { GalleryModalComponent, GalleryModalInput } from '../shared/gallery-modal.component';
import { ISharepointModalOptions, SharepointModalComponent } from '../sharepoint-browser-modal/sharepoint-modal.component';
import { SharepointService } from '../services/sharepoint/sharepoint.service';

@Component({
    selector: 'app-job-modal',
    templateUrl: 'job-modal.component.html'
})
export class JobModalComponent extends ModalFormComponent<Job> implements OnInit {

    @Input() jobReadOnly: boolean = false;

    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    @ViewChild('galleryModal', { static: true }) galleryModal: GalleryModalComponent;
    @ViewChild('sharepointModal', { static: true }) sharepointModal: SharepointModalComponent

    public windowState: WindowState = "default";

    customers: CustomerModel[];
    addresses: AddressModel[];
    operators: OperatorModel[];
    attachmentsPreventivo: Array<FileInfo> = [];
    attachmentsRilievo: Array<FileInfo> = [];
    attachmentsAltro: Array<FileInfo> = [];
    messages: MessageReadModel[];
    user: User;
    currentOperator: OperatorModel;
    unreadMessages: number;
    targetOperatorsArray: number[];
    readonly isOperator: boolean;
    readonly isAdmin: boolean;
    readonly JobAttachmentType = JobAttachmentType;

    readonly states = listEnum<JobStatus>(JobStatus);

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/jobs`;
    pathImage = `${ApiUrls.baseAttachmentsUrl}/`;
    uploadSaveUrl = `${this._baseUrl}/job-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/job-attachment/remove-file`;
    cannotBrowseSharepoint: boolean = false;

    contactName: string;
    contactReference: string;

    constructor(
        security: SecurityService,
        private readonly _customersService: CustomerService,
        private readonly _service: JobsService,
        messageBox: MessageBoxService,
        private readonly _addressesService: AddressesService,
        private readonly _operatorsService: OperatorsService,
        private readonly _user: UserService,
        private readonly _messagesService: MessagesService,
        private readonly _sharepoint: SharepointService
    ) {
        super(messageBox);
        this.isOperator = security.isAuthorized(Role.Operator);
        this.isAdmin = security.isAuthorized(Role.Administrator);
    }

    ngOnInit() {
        this._getData();
        this._getOperators();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
    }

    onDateChange() {
        this.options.year = this.options.date?.getFullYear();
    }

    override open(job: Job) {
        const result = super.open(job);

        this.attachmentsPreventivo = [];
        this.attachmentsRilievo = [];
        this.attachmentsAltro = [];
        this.contactName = null;
        this.contactReference = null;

        if (job.attachments != null) {
            this.options.attachments.forEach(element => {
                if (element.displayName != null && element.fileName != null) {
                    const fileInfo: FileInfo = { name: element.displayName };
                    switch (element.type) {
                        case JobAttachmentType.Preventivo:
                            this.attachmentsPreventivo.push(fileInfo);
                            break;
                        case JobAttachmentType.Rilievo:
                            this.attachmentsRilievo.push(fileInfo);
                            break;
                        default:
                            this.attachmentsAltro.push(fileInfo);
                            break;
                    }
                }
            });
        }
        if (job.sharepointFolder == null) {
            this.options.sharepointFolder = this._sharepoint.rootItemId;
        }
        this.cannotBrowseSharepoint = (job.sharepointFolder == null || job.sharepointFolder == this._sharepoint.rootItemId);

        this.updateUnreadCounter();
        this.readAddresses();
        return result;
    }

    onCustomerChange() {
        const selectedCustomer = this.customers.find(e => e.id == this.options.customerId);
        this.addresses = selectedCustomer?.addresses ?? [];
        if (selectedCustomer != undefined) {
            const selectedAddress: AddressModel = selectedCustomer.addresses.find(x => x.isMainAddress == true);
            if (selectedAddress != undefined) {
                this.options.addressId = selectedAddress.id;
            }
        }
    }

    onAddressChange() {
        const selectedAddress = this.addresses.find(e => e.id == this.options.addressId);
        if (selectedAddress != undefined) {
            this.contactName = selectedAddress.contactName;
            this.contactReference = selectedAddress.contactReference;
        }
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        if (this.options.expirationDate == null && this.options.mandatoryDate == null) {
            this._messageBox.error('Inserire almeno una tra Data Scadenza e Data Obbligatoria.');
            return false;
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
                        this.readAddresses();
                        this._messageBox.success(`Cliente ${request.name} creato`);
                    }),
                    tap(() => this._getData())
                )
                .subscribe()
        );
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

    browseSharepointPath(path: string, folderName: string, browseMode: boolean) {
        const options: ISharepointModalOptions = {
            path,
            folderName,
            browseMode
        };

        this.sharepointModal.startPath = this.options.sharepointFolder;
        this._subscriptions.push(
            this.sharepointModal.open(options)
                .pipe(
                    filter(e => e),
                    tap(() => {
                        const selectedFolderName = this.sharepointModal.navigationMenuItems.slice(-1)[0]?.text;
                        this.options.sharepointFolder = this.sharepointModal.currentPath;
                        this.options.sharepointFolderName = selectedFolderName || null;
                        this.cannotBrowseSharepoint = (this.options.sharepointFolder == null || this.options.sharepointFolder == this._sharepoint.rootItemId);
                    })
                )
                .subscribe()
        );
    }

    addNewAddress(address: AddressModel) {
        if (this.options.customerId !== null) address.customerId = this.options.customerId;
        this._subscriptions.push(
            this._addressesService.createAddress(address)
                .pipe(
                    tap(e => {
                        this.readAddresses();
                        this.options.addressId = e.id;
                        this._messageBox.success(`Indirizzo creato con successo`);
                    })
                )
                .subscribe()
        );
    }

    readAddresses() {
        this._subscriptions.push(
            this._addressesService.getCustomerAddresses(this.options.customerId)
                .pipe(
                    map(e => {
                        this.addresses = e;
                    }),
                    tap(() => {
                        this.onAddressChange();
                    })
                )
                .subscribe()
        );
    }

    deleteJob() {
        this._messageBox.confirm(`Sei sicuro di voler cancellare la commessa?`, 'Conferma l\'azione').subscribe(result => {
            if (result == true) {
                this._subscriptions.push(
                    this._service.delete(this.options.id)
                        .pipe(
                            tap(e => this._messageBox.success(`Commessa cancellata con successo`)),
                            tap(() => this.dismiss())
                        )
                        .subscribe()
                );
            }
        });
    }

    downloadAttachment(fileName: string) {
        const attachment = this.options.attachments
            .find(e => e.displayName === fileName);
        const url = `${this._baseUrl}/job-attachment/download-file/${attachment.fileName}/${attachment.displayName}`;

        window.open(url);
    }

    public AttachmentExecutionSuccess(e: SuccessEvent, type: JobAttachmentType): void {
        const file = e.response.body as JobAttachmentUploadFileModel;
        if (file != null) {
            let jobAttachment = new JobAttachmentModel(0, file.originalFileName, file.fileName, this.options.id, type);
            this.options.attachments.push(jobAttachment);
        } else {
            const deletedFile = e.files[0].name;
            this.options.attachments.findAndRemove(a => a.displayName === deletedFile && a.type === type);
        }
    }

    getAttachmentsByType(type: JobAttachmentType): JobAttachmentModel[] {
        return this.options.attachments?.filter(a => a.type === type) ?? [];
    }

    openImage(index: number, type: JobAttachmentType) {
        const album = this.getAttachmentsByType(type)
            .map(a => a.isImage ? this.pathImage + a.fileName : 'assets/document.jpg');
        const galleryOptions = new GalleryModalInput(album, index);
        this.galleryModal.open(galleryOptions).subscribe();
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
    }

    createMessage() {
        const today = new Date();
        const message = new MessageModel(0, today, null, this.currentOperator.id, this.options.id, null, null, null, false);
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
                        message.isRead = !message.isRead;
                        this.updateUnreadCounter();
                        this._messageBox.success('Commento aggiornato');
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

    isMyComment(targetOperators: string): boolean {
        if (targetOperators == null || targetOperators.length == 0) return false;
        return targetOperators.includes(this.currentOperator.name);
    }
    
    disableEditOrDeleteMessage(message: MessageReadModel): boolean {
        if (!this.isOperator && message.isFromApp) return false;
        if (message.operatorId != this.currentOperator.id) return true;
        return false;
    }
}
