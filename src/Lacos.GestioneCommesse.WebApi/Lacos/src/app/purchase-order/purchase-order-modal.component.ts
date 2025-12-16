import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobsService } from '../services/jobs/jobs.service';
import { State, process } from '@progress/kendo-data-query';
import { IJobReadModel, Job, SelectableJob } from '../services/jobs/models';
import { getToday, listEnum } from '../services/common/functions';
import { ApiUrls } from '../services/common/api-urls';
import { SupplierModel } from '../shared/models/supplier.model';
import { SupplierService } from '../services/supplier.service';
import { PurchaseOrder, PurchaseOrderExpense, PurchaseOrderItem, PurchaseOrderStatus } from '../services/purchase-orders/models';
import { PurchaseOrderItemModalComponent } from './purchase-order-item-modal.component';
import { filter, map, switchMap, tap } from 'rxjs';
import { DataStateChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { FileInfo, SuccessEvent } from '@progress/kendo-angular-upload';
import { PurchaseOrderAttachmentUploadFileModel } from '../services/purchase-orders/purchage-order-attachment-upload-file.model';
import { PurchaseOrderAttachmentModel } from '../services/purchase-orders/purchase-order-attachment.model';
import { SupplierModalComponent } from '../supplier-modal/supplier-modal.component';
import { MessageModalComponent } from '../messages/message-modal.component';
import { MessageModalOptions, MessageModel, MessageReadModel } from '../services/messages/models';
import { Role, User } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { UserService } from '../services/security/user.service';
import { MessagesService } from '../services/messages/messages.service';
import { SecurityService } from '../services/security/security.service';
import { GalleryModalComponent, GalleryModalInput } from '../shared/gallery-modal.component';
import { ActivityTypesService } from '../services/activityTypes.service';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { PurchaseOrderExpenseModalComponent } from './purchase-order-expense-modal.component';

@Component({
    selector: 'app-purchase-order-modal',
    templateUrl: 'purchase-order-modal.component.html'
})
export class PurchaseOrderModalComponent extends ModalFormComponent<PurchaseOrderModalOptions> implements OnInit {

    @ViewChild('purchaseOrderItemModal', { static: true }) purchaseOrderItemModal: PurchaseOrderItemModalComponent;
    @ViewChild('purchaseOrderExpenseModal', { static: true }) purchaseOrderExpenseModal: PurchaseOrderExpenseModalComponent;
    @ViewChild('supplierModal', { static: true }) supplierModal: SupplierModalComponent;
    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    @ViewChild('galleryModal', { static: true }) galleryModal: GalleryModalComponent;

    jobsList: SelectableJob[];
    originalJobsList: SelectableJob[];
    job: Job;
    jobReadonly: boolean;

    get selectedJobsList(): SelectableJob[] {
        if (!this.jobsList || !this.options?.purchaseOrder?.jobs) {
            return [];
        }
        return this.jobsList.filter(j => this.options.purchaseOrder.jobs.includes(j.id));
    }
    status: PurchaseOrderStatus;
    selectedJob: SelectableJob;
    suppliers: SupplierModel[];
    gridStateItem: State = {
        skip: 0,
        take: 30,
        sort: [
            { field: 'productName', dir: 'asc' }
        ]
    };
    gridDataItems: GridDataResult;
    gridStateExpense: State = {
        skip: 0,
        take: 30,
        sort: [
            { field: 'jobCode', dir: 'asc' },
            { field: 'note', dir: 'asc' }
        ]
    };
    gridDataExpenses: GridDataResult;

    activityTypes: ActivityTypeModel[];
    userAttachments: Array<FileInfo> = [];
    adminAttachments: Array<FileInfo> = [];
    messages: MessageReadModel[];
    user: User;
    currentOperator: OperatorModel;
    unreadMessages: number;
    isAdmin: boolean;
    album: string[] = [];
    targetOperatorsArray: number[];

    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/purchase-orders`;
    uploadSaveUrl = `${this._baseUrl}/purchase-order-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/purchase-order-attachment/remove-file`;

    readonly states = listEnum<PurchaseOrderStatus>(PurchaseOrderStatus);

    constructor(
        private security: SecurityService,
        messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
        private readonly _suppliersService: SupplierService,
        private readonly _operatorsService: OperatorsService,
        private readonly _user: UserService,
        private readonly _activityTypesService: ActivityTypesService,
        private readonly _messagesService: MessagesService
    ) {
        super(messageBox);
        this.isAdmin = security.isAuthorized(Role.Administrator);
    }

    ngOnInit() {
        this._getActivityTypes();
        this._getSuppliers();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
    }

    onSupplierChange() {
        this.options.purchaseOrder.supplierName = this.suppliers.find(e => e.id == this.options.purchaseOrder.supplierId)?.name ?? '';
    }

    addItem() {
        const item = new PurchaseOrderItem(0, this.options.purchaseOrder.id, null, null, null, 1);

        this._subscriptions.push(
            this.purchaseOrderItemModal.open(item)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterProductAdded(item))
                )
                .subscribe()
        );
    }

    editItem(item: PurchaseOrderItem) {
        const updatedItem = item.clone();

        this._subscriptions.push(
            this.purchaseOrderItemModal.open(updatedItem)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterProductUpdated(item, updatedItem))
                )
                .subscribe()
        );
    }

    removeItem(item: PurchaseOrderItem) {
        this._subscriptions.push(
            this._messageBox.confirm(`Sei sicuro di voler rimuovere il prodotto ${item.productName}?`)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterProductRemoved(item))
                )
                .subscribe()
        );
    }

    onDataItemsStateChange(state: DataStateChangeEvent | State) {
        this.gridStateItem = state;
        this.gridDataItems = process(this.options.purchaseOrder.items, this.gridStateItem);
    }
    
    addExpense() {
        const expense = new PurchaseOrderExpense(0, this.options.purchaseOrder.id, 
            this.selectedJobsList.first().id, this.selectedJobsList.first().code, null, 1, 0);

        this._subscriptions.push(
            this.purchaseOrderExpenseModal.open(expense)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterExpenseAdded(expense))
                )
                .subscribe()
        );
    }

    editExpense(expense: PurchaseOrderExpense) {
        const updatedExpense = expense.clone();

        this._subscriptions.push(
            this.purchaseOrderExpenseModal.open(updatedExpense)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterExpenseUpdated(expense, updatedExpense))
                )
                .subscribe()
        );
    }

    removeExpense(expense: PurchaseOrderExpense) {
        this._subscriptions.push(
            this._messageBox.confirm(`Sei sicuro di voler rimuovere la voce ${expense.note}?`)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterExpenseRemoved(expense))
                )
                .subscribe()
        );
    }

    onDataExpensesStateChange(state: DataStateChangeEvent | State) {
        this.gridStateExpense = state;
        this.gridDataExpenses = process(this.options.purchaseOrder.expenses, this.gridStateExpense);
    }

    override open(options: PurchaseOrderModalOptions) {
        const result = super.open(options);
        this._getJobs();

        this.userAttachments = [];
        this.album = [];
        if (options.purchaseOrder.attachments != null) {
            this.options.purchaseOrder.userAttachments.forEach(element => {
                if (element.displayName != null && element.fileName != null) {
                    this.userAttachments.push({ name: element.displayName });
                    if (element.isImage) this.album.push(this.imagesUrl + element.fileName);
                    if (!element.isImage) this.album.push("assets/document.jpg");
                }
            });
        }

        this.adminAttachments = [];
        if (options.purchaseOrder.adminAttachments != null) {
            this.options.purchaseOrder.adminAttachments.forEach(element => {
                if (element.displayName != null && element.fileName != null) {
                    this.adminAttachments.push({ name: element.displayName });
                }
            });
        }

        this.jobReadonly = this.user.role != Role.Administrator;
        this.status = options.purchaseOrder.status;
        this.onDataItemsStateChange(this.gridStateItem);
        this.onDataExpensesStateChange(this.gridStateExpense);
        this.updateUnreadCounter();

        return result;
    }

    onJobsFilterChange(value: string) {
        this.jobsList = this.originalJobsList.filter(j =>
            j.fullName.toLowerCase().includes(value.toLowerCase())
        );
    }

    onJobsClick() {
        this.jobsList = [...this.originalJobsList];
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
            this._activityTypesService.readActivityTypesListPO()
                .pipe(
                    tap(e => this.activityTypes = e)
                )
                .subscribe()
        );
    }

    createSupplier() {
        const request = new SupplierModel();

        this._subscriptions.push(
            this.supplierModal.open(request)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._suppliersService.createSupplier(request)),
                    tap(e => {
                        this.options.purchaseOrder.supplierId = e.id;
                        this._messageBox.success(`Fornitore ${request.name} creato`);
                    }),
                    tap(() => this._getSuppliers())
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
                        if (this.options) if (this.options.purchaseOrder.supplierId) {
                            this.onSupplierChange();
                        }
                    })
                )
                .subscribe()
        );
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
                { field: 'year', dir: 'asc' },
                { field: 'number', dir: 'asc' }
            ]
        };

        this._subscriptions.push(
            this._jobsService.getSelectable(state)
                .pipe(
                    tap(e => {
                        this.originalJobsList = e.data as SelectableJob[];
                        this.jobsList = [...this.originalJobsList];
                    })
                )
                .subscribe()
        );
    }

    private _afterProductAdded(item: PurchaseOrderItem) {
        this.options.purchaseOrder.items.push(item);
        this.onDataItemsStateChange(this.gridStateItem);

        this._messageBox.success(`Prodotto aggiunto`);
    }

    private _afterProductUpdated(originalItem: PurchaseOrderItem, updatedItem: PurchaseOrderItem) {
        this.options.purchaseOrder.items.replace(originalItem, updatedItem);
        this.onDataItemsStateChange(this.gridStateItem);

        this._messageBox.success(`Prodotto aggiornato`);
    }

    private _afterProductRemoved(item: PurchaseOrderItem) {
        this.options.purchaseOrder.items.remove(item);
        this.onDataItemsStateChange(this.gridStateItem);

        this._messageBox.success(`Prodotto rimosso`);
    }

    private _afterExpenseAdded(expense: PurchaseOrderExpense) {
        this.options.purchaseOrder.expenses.push(expense);
        this.onDataExpensesStateChange(this.gridStateItem);

        this._messageBox.success(`Voce costo aggiunta`);
    }

    private _afterExpenseUpdated(originalExpense: PurchaseOrderExpense, updatedExpense: PurchaseOrderExpense) {
        this.options.purchaseOrder.expenses.replace(originalExpense, updatedExpense);
        this.onDataExpensesStateChange(this.gridStateItem);

        this._messageBox.success(`Voce costo aggiornata`);
    }

    private _afterExpenseRemoved(expense: PurchaseOrderExpense) {
        this.options.purchaseOrder.expenses.remove(expense);
        this.onDataExpensesStateChange(this.gridStateItem);

        this._messageBox.success(`Voce costo rimossa`);
    }

    downloadAttachment(fileName: string) {
        const attachment = this.options.purchaseOrder.attachments
            .find(e => e.displayName === fileName);
        const url = `${this._baseUrl}/purchase-order-attachment/download-file/${attachment.fileName}/${attachment.displayName}`;

        window.open(url);
    }

    public AttachmentExecutionSuccess(e: SuccessEvent): void {
        const file = e.response.body as PurchaseOrderAttachmentUploadFileModel;
        if (file != null) {
            let purchaseOrderAttachmentModal = new PurchaseOrderAttachmentModel(0, file.originalFileName, file.fileName, this.options.purchaseOrder.id, false);
            this.options.purchaseOrder.attachments.push(purchaseOrderAttachmentModal);
            this.options.purchaseOrder.userAttachments.push(purchaseOrderAttachmentModal);
        } else {
            const deletedFile = e.files[0].name;
            this.options.purchaseOrder.attachments.findAndRemove(e => e.displayName === deletedFile);
            this.options.purchaseOrder.userAttachments.findAndRemove(e => e.displayName === deletedFile);
        }
    }

    public AttachmentExecutionSuccessAdmin(e: SuccessEvent): void {
        const file = e.response.body as PurchaseOrderAttachmentUploadFileModel;
        if (file != null) {
            let purchaseOrderAttachmentModal = new PurchaseOrderAttachmentModel(0, file.originalFileName, file.fileName, this.options.purchaseOrder.id, true);
            this.options.purchaseOrder.attachments.push(purchaseOrderAttachmentModal);
            this.options.purchaseOrder.adminAttachments.push(purchaseOrderAttachmentModal);
        } else {
            const deletedFile = e.files[0].name;
            this.options.purchaseOrder.attachments.findAndRemove(e => e.displayName === deletedFile);
            this.options.purchaseOrder.adminAttachments.findAndRemove(e => e.displayName === deletedFile);
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
        if (this.options.purchaseOrder.id == 0) {
            this._messageBox.info("Prima di creare il nuovo commento è necessario salvare l'elemento corrente");
            return;
        }
        this.createMessage();
        // this._subscriptions.push(
        //     this._messagesService.getElementTargetOperators(this.currentOperator.id, this.options.purchaseOrder.id, "O")
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
        const message = new MessageModel(0, today, null, this.currentOperator.id, null, null, null, this.options.purchaseOrder.id, false);
        const options = new MessageModalOptions(message, true, true, this.targetOperatorsArray);

        this._subscriptions.push(
            this.messageModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._messagesService.create(message, options.targetOperators.join(","))),
                    tap(e => {
                        var msg = new MessageReadModel(e.id, e.date, e.note, e.operatorId, this.currentOperator.name, e.jobId, e.activityId, e.ticketId, e.purchaseOrderId, "", true, [], false, null);
                        this.options.purchaseOrder.messages.push(msg);
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
        const originalMsg = this.options.purchaseOrder.messages.find(e => e.id == message.id);
        originalMsg.date = message.date;
        originalMsg.note = message.note;
        this.updateUnreadCounter();
        //this._read();
    }

    editMessage(message: MessageReadModel) {
        this.targetOperatorsArray = [];
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
                                this.options.purchaseOrder.messages.remove(message);
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
        this.unreadMessages = this.options.purchaseOrder.messages.count(e => !e.isRead);
    }

    openImage(index: number) {
        const options = new GalleryModalInput(this.album, index);
        this.galleryModal.open(options).subscribe();
    }

}

export class PurchaseOrderModalOptions {

    constructor(
        readonly purchaseOrder: PurchaseOrder
    ) {
    }

}
