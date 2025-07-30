import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridComponent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { JobsService } from '../services/jobs/jobs.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { JobModalComponent } from './job-modal.component';
import { IJobReadModel, Job, JobCopy, JobStatus, jobStatusNames } from '../services/jobs/models';
import { getToday } from '../services/common/functions';
import { ActivityModalComponent, ActivityModalOptions } from '../activities/activity-modal.component';
import { Activity, ActivityStatus } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { Router } from '@angular/router';
import { PurchaseOrder, PurchaseOrderStatus } from '../services/purchase-orders/models';
import { PurchaseOrderModalComponent, PurchaseOrderModalOptions } from '../purchase-order/purchase-order-modal.component';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { JobsAttachmentsModalComponent } from '../jobs/jobs-attachments-modal.component';
import { StorageService } from '../services/common/storage.service';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { UserService } from '../services/security/user.service';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';
import { User } from '../services/security/models';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';
import { JobStatusPipe } from '../shared/pipes/job-status.pipe';

@Component({
    selector: 'app-jobs-completed',
    templateUrl: 'jobs-completed.component.html'
})
export class JobsCompletedComponent extends BaseComponent implements OnInit {

    @ViewChild('jobModal', { static: true })
    jobModal: JobModalComponent;

    @ViewChild('activityModal', { static: true })
    activityModal: ActivityModalComponent;

    @ViewChild('purchaseOrderModal', { static: true })
    purchaseOrderModal: PurchaseOrderModalComponent;

    @ViewChild('grid', { static: true })
    grid: GridComponent;

    @ViewChild('jobsAttachmentsModal', { static: true })
    jobsAttachmentsModal: JobsAttachmentsModalComponent;

    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;

    user: User;
    currentOperator: OperatorModel;
    screenWidth: number;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
                {
                    filters: [JobStatus.Billing, JobStatus.Completed]
                        .map(e => ({ field: 'status', operator: 'eq', value: e })),
                    logic: 'or'
                }
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'date', dir: 'desc' }, { field: 'code', dir: 'desc' }]
    };

    readonly jobStatusNames = jobStatusNames;
    private cellArgs: CellClickEvent;

    constructor(
        private readonly _service: JobsService,
        private readonly _serviceActivity: ActivitiesService,
        private readonly _purchaseOrdersService: PurchaseOrdersService,
        private readonly _customerService: CustomerService,
        private readonly _user: UserService,
        private readonly _operatorsService: OperatorsService,
        private readonly _messageBox: MessageBoxService,
        private router: Router,
        private readonly _storageService: StorageService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this._read();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
        this.updateScreenSize();
    }

    @HostListener('window:resize', ['$event'])
    onResize(event: Event): void {
        this.updateScreenSize();
    }

    private updateScreenSize(): void {
        this.screenWidth = window.innerWidth - 44;
        if (this.screenWidth > 1876) this.screenWidth = 1876;
        if (this.screenWidth < 1400) this.screenWidth = 1400;
    }


    dataStateChange(state: State) {
        this.gridState = state;
        this._saveState();
        this._read();
    }

    private _resumeState() {
        const savedState = this._storageService.get<State>(window.location.hash, true);
        if (savedState == null) return;
        this.gridState = savedState;
    }

    private _saveState() {
        this._storageService.save(this.gridState, window.location.hash, true);
    }

    create() {
        const today = new Date();
        const job = new Job(0, null, today.getFullYear(), today, null, null, null, null, false, JobStatus.Pending, null, null, null, null, null, [], []);

        this._subscriptions.push(
            this.jobModal.open(job)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(job)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    edit(job: IJobReadModel) {
        this._subscriptions.push(
            this._service.get(job.id)
                .pipe(
                    switchMap(e => this.jobModal.open(e)),
                    tap(e => !e && this._read()),
                    filter(e => e),
                    switchMap(() => this._service.update(this.jobModal.options)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    onDblClick(): void {
        if (!this.cellArgs.isEdited) {
            this.router.navigate(['/job-details'], { queryParams: { jobId: this.cellArgs.dataItem.id } });
        }
    }

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    createActivity(job: IJobReadModel) {
        const activity = new Activity(0, ActivityStatus.Pending, null, null, null, null, job.id, null, null, null, null, null, null, 
            "In attesa", "In corso", "Pronto", "Completata", false, false, [], []);
        const options = new ActivityModalOptions(activity);

        this._subscriptions.push(
            this.activityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._serviceActivity.create(activity)),
                    tap(() => this._afterActivityCreated(job.id)),

                )
                .subscribe()
        );
    }

    openCustomer(customerId: number): void {
        this._subscriptions.push(
            this._customerService.getCustomer(customerId)
                .pipe(
                    map(e => {
                        return Object.assign(new CustomerModel(), e);
                    }),
                    switchMap(e => this.customerModal.open(e)),
                    filter(e => e),
                    map(() => this.customerModal.options),
                    switchMap(e => this._customerService.updateCustomer(e, customerId)),
                    map(() => this.customerModal.options),
                    tap(e => this._messageBox.success(`Cliente ${e.name} aggiornato`)),
                    tap(() => this._read())
                )
                .subscribe()
        );
    }

    askRemove(job: IJobReadModel) {
        const text = `Sei sicuro di voler rimuovere la commessa ${job.code}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(job.id)),
                    tap(() => this._afterRemoved(job))
                )
                .subscribe()
        );
    }

    createPurchaseOrder(job: IJobReadModel) {
        const today = getToday();
        const order = new PurchaseOrder(0, null, today.getFullYear(), today, null, null, PurchaseOrderStatus.Pending, null, null, null,
            this.currentOperator.id, [job.id], [], [], [], [], []);
        const options = new PurchaseOrderModalOptions(order);

        this._subscriptions.push(
            this.purchaseOrderModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._purchaseOrdersService.create(order)),
                    tap(() => this._afterPurchaseOrderCreated(job.id)),

                )
                .subscribe()
        )
    }

    openAttachments(job: IJobReadModel) {
        this._subscriptions.push(
            this.jobsAttachmentsModal.open([job.id, 0, 0])
                .pipe(
                    filter(e => e)
                )
                .subscribe()
        );
    }

    private _afterActivityCreated(jobId: number) {
        this._messageBox.success('Attività creata.');
        this.router.navigate(['/activities'], { queryParams: { jobId: jobId } });
    }

    private _afterPurchaseOrderCreated(jobId: number) {
        this._messageBox.success('Ordine d\'acquisto creato');

        this.router.navigate(['/purchase-orders'], { queryParams: { jobId: jobId } });
    }

    protected _read() {
        this._subscriptions.push(
            this._service.readCurrentJobs(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    private _afterSaved(job: Job) {
        this._messageBox.success(`Commessa ${job.code} salvata.`);

        this._read();
    }

    private _afterRemoved(job: IJobReadModel) {
        const text = `Commessa ${job.code} rimossa.`;

        this._messageBox.success(text);

        this._read();
    }

    readonly rowCallback = (context: RowClassArgs) => {
        const job = context.dataItem as IJobReadModel;

        switch (true) {
            case job.status === JobStatus.Completed:
                return { 'job-completed': true };
            case job.status === JobStatus.InProgress:
                return { 'job-inprogress': true };
            case job.status === JobStatus.Pending:
                return { 'job-pending': true };
            case job.status === JobStatus.Billing:
                return { 'job-billing': true };
            case job.status === JobStatus.Billed:
                return { 'job-billed': true };
            case job.status === JobStatus.Suspended:
                return { 'job-suspended': true };
            default:
                return {};
        }
    };

    protected _getCurrentOperator(userId: number) {
        this._subscriptions.push(
            this._operatorsService.getOperatorByUserId(userId)
                .pipe(
                    tap(e => this.currentOperator = e)
                )
                .subscribe()
        );
    }

    exportToExcel(): void {
        const options = this.getExportOptions();
        const workbook = new Workbook(options);
        workbook.toDataURL().then((dataURL) => {
            saveAs(dataURL, 'commesse_completate.xlsx');
        });
    }

    private getExportOptions(): any {
        return {
            sheets: [{
                columns: [
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true }
                ],
                title: 'Commesse',
                rows: [
                    {
                        cells: [
                            { value: 'Data', bold: true },
                            { value: 'Data Scadenza', bold: true },
                            { value: 'Cliente', bold: true },
                            { value: 'Commessa', bold: true },
                            { value: 'Stato', bold: true },
                            { value: 'Riferimento', bold: true },
                            { value: 'Responsabile Comm', bold: true },
                            { value: 'Note', bold: true }
                        ]
                    },
                    ...this.data.data.map((item: any) => ({
                        cells: [
                            { value: item.date, format: 'dd/MM/yyyy' },
                            { value: item.expirationDate, format: 'dd/MM/yyyy' },
                            { value: item.customer },
                            { value: item.code },
                            { value: new JobStatusPipe().transform(item.status) },
                            { value: item.reference },
                            { value: item.referentName },
                            { value: item.description }
                        ]
                    }))
                ]
            }]
        };
    }
}
