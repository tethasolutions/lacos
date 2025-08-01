import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Activity, ActivityStatus, IActivityReadModel, activityStatusNames } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivatedRoute, Params } from '@angular/router';
import { ActivityModalComponent, ActivityModalOptions } from './activity-modal.component';
import { JobsService } from '../services/jobs/jobs.service';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';
import { UserService } from '../services/security/user.service';
import { User } from '../services/security/models';
import { CustomerService } from '../services/customer.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { CustomerModel } from '../shared/models/customer.model';
import { StorageService } from '../services/common/storage.service';
import { JobsAttachmentsModalComponent } from '../jobs/jobs-attachments-modal.component';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';
import { DependenciesModalComponent } from '../dependencies/dependencies-modal.component';
import { PurchaseOrderStatus } from '../services/purchase-orders/models';
import { Job } from '../services/jobs/models';
import { JobLateNotificationComponent } from '../jobs/job-late-notification.component';

@Component({
    selector: 'app-activities',
    templateUrl: 'activities.component.html'
})
export class ActivitiesComponent extends BaseComponent implements OnInit {

    @Input() viewExportExcel: boolean = true;

    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('jobsAttachmentsModal', { static: true }) jobsAttachmentsModal: JobsAttachmentsModalComponent;
    @ViewChild('dependenciesModal', { static: false }) dependenciesModal: DependenciesModalComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
                this._buildStatusFilter(),
                this._buildJobIdFilter(),
                this._buildTypeIdFilter(),
                this._buildReferentIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'jobIsInLate', dir: 'desc' }, { field: 'startDate', dir: 'asc' }, { field: 'expirationDate', dir: 'asc' }]
    };

    private _jobId: number;
    private _typeId: number;
    private _referentId: number;
    private cellArgs: CellClickEvent;
    user: User;
    currentOperator: OperatorModel;
    job: Job;
    screenWidth: number;
    lateJobsToNotify: any[] = [];

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _service: ActivitiesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _route: ActivatedRoute,
        private readonly _user: UserService,
        private readonly _customerService: CustomerService,
        private readonly _operatorsService: OperatorsService,
        private readonly _storageService: StorageService,
        private readonly _jobsService: JobsService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this._subscribeRouteParams();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
        this.updateScreenSize();

        if (this._jobId) {
            this._subscriptions.push(
                this._jobsService.get(this._jobId)
                    .pipe(
                        tap(e => this.job = e)
                    )
                    .subscribe()
            );
        }

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

    private _resumeState() {
        const savedState = this._storageService.get<State>(window.location.hash, true);
        if (savedState == null) return;
        this.gridState = savedState;
    }

    private _saveState() {
        this._storageService.save(this.gridState, window.location.hash, true);
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._saveState();
        this._read();
    }

    askRemove(activity: IActivityReadModel) {
        const text = `Sei sicuro di voler rimuovere l'attività ${activity.number}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(activity.id)),
                    tap(() => this._afterRemoved(activity))
                )
                .subscribe()
        );
    }

    create() {
        const activity = new Activity(0, ActivityStatus.Pending, null, null, null, null, this._jobId, null, null, null, null, null, null,
            "In attesa", "In corso", "Pronto", "Completata", false, false, [], []);
        if (this.job) {
            activity.addressId = this.job.addressId;
        }
        const options = new ActivityModalOptions(activity);

        this._subscriptions.push(
            this.activityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(activity)),
                    tap(() => this._afterActivityCreated())
                )
                .subscribe()
        );
    }

    edit(activity: IActivityReadModel) {
        this._subscriptions.push(
            this._service.get(activity.id)
                .pipe(
                    map(e => new ActivityModalOptions(e)),
                    switchMap(e => this.activityModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.activityModal.options.activity)),
                    tap(() => this._afterActivityUpdated())
                )
                .subscribe()
        );
    }

    onDblClick(): void {
        if (!this.cellArgs.isEdited) {
            this._subscriptions.push(
                this._service.get(this.cellArgs.dataItem.id)
                    .pipe(
                        map(e => new ActivityModalOptions(e)),
                        switchMap(e => this.activityModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._service.update(this.activityModal.options.activity)),
                        tap(() => this._afterActivityUpdated())
                    )
                    .subscribe()
            )
        }
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
                    tap(() => this._afterActivityUpdated())
                )
                .subscribe()
        );
    }

    openAttachments(jobId: number, activityId: number) {
        this._subscriptions.push(
            this.jobsAttachmentsModal.open([0, activityId, 0])
                .pipe(
                    filter(e => e)
                )
                .subscribe()
        );
    }

    openDependencies(jobId: number, activityId: number, canHaveDependencies: boolean) {
        this.dependenciesModal.jobId = jobId;
        this.dependenciesModal.activityId = activityId;
        this.dependenciesModal.readonly = true;
        this.dependenciesModal.canHaveDependencies = canHaveDependencies;
        this.dependenciesModal.open();
    }

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }
    readonly rowCallback = (context: RowClassArgs) => {
        const activity = context.dataItem as IActivityReadModel;
        const classes: { [key: string]: boolean } = {};

        // Activity status classes
        if (activity.status === ActivityStatus.Completed) {
            classes['activity-completed'] = true;
        } else if (activity.status === ActivityStatus.Pending) {
            classes['activity-pending'] = true;
        } else if (activity.status === ActivityStatus.InProgress) {
            classes['activity-in-progress'] = true;
        } else if (activity.status === ActivityStatus.Ready) {
            classes['activity-ready'] = true;
        } else if (
            activity.status != ActivityStatus.Completed &&
            !!activity.expirationDate &&
            new Date(activity.expirationDate).addDays(1).isPast()
        ) {
            classes['activity-expired'] = true;
        }

        // Purchase order status classes
        if (activity.purchaseOrderStatus === PurchaseOrderStatus.Completed) {
            classes['purchase-order-completed'] = true;
        } else if (activity.purchaseOrderStatus === PurchaseOrderStatus.Pending) {
            classes['purchase-order-pending'] = true;
        } else if (activity.purchaseOrderStatus === PurchaseOrderStatus.Ordered) {
            classes['purchase-order-ordered'] = true;
        } else if (activity.purchaseOrderStatus === PurchaseOrderStatus.Partial) {
            classes['purchase-order-partial'] = true;
        } else if (activity.purchaseOrderStatus === PurchaseOrderStatus.Canceled) {
            classes['purchase-order-canceled'] = true;
        }

        return classes;
    };

    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => {
                        this.data = e;
                        this.checkLateJobsToNotify();
                    })
                )
                .subscribe()
        );
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

    private _afterActivityCreated() {
        this._messageBox.success('Attività creata.')

        this._read();
    }

    private _afterActivityUpdated() {
        this._messageBox.success('Attività aggiornata.')

        this._read();
    }

    private _subscribeRouteParams() {
        this._route.queryParams
            .pipe(
                // switchMap(e =>
                //     +e['jobId']
                //         ? this._jobsService.get(+e['jobId'])
                //         : of(void 0)
                // ),                
                tap(e => this._setParams(e))
            )
            .subscribe();
    }

    private _afterRemoved(activity: IActivityReadModel) {
        const text = `Attività ${activity.number} rimossa.`;

        this._messageBox.success(text);

        this._read();
    }

    private _buildJobIdFilter() {
        const that = this;

        return {
            field: 'jobId',
            get operator() {
                return that._jobId
                    ? 'eq'
                    : 'isnotnull'
            },
            get value() {
                return that._jobId;
            }
        };
    }

    private _buildTypeIdFilter() {
        const that = this;

        return {
            field: 'typeId',
            get operator() {
                return that._typeId
                    ? 'eq'
                    : 'isnotnull'
            },
            get value() {
                return that._typeId;
            }
        };
    }

    private _buildReferentIdFilter() {
        const that = this;

        //if (that._referentId == null) return {};

        return {
            field: 'referentId',
            get operator() {
                return that._referentId
                    ? 'eq'
                    : 'neq'
            },
            get value() {
                return that._referentId
                    ? that._referentId
                    : 0;
            }
        };
    }

    private _buildStatusFilter() {
        const that = this;

        return {
            get field() {
                return that._jobId
                    ? 'id'
                    : undefined
            },
            get operator() {
                return that._jobId
                    ? 'isnotnull'
                    : undefined
            },
            get filters() {
                return that._jobId
                    ? undefined
                    : [ActivityStatus.Pending, ActivityStatus.InProgress, ActivityStatus.Ready]
                        .map(e => ({ field: 'status', operator: 'eq', value: e }))
            },
            logic: 'or'
        };
    }

    private _setParams(params: Params) {
        this._jobId = isNaN(+params['jobId']) ? null : +params['jobId'];
        this._typeId = isNaN(+params['typeId']) ? null : +params['typeId'];
        this._referentId = isNaN(+params['referentId']) ? null : +params['referentId'];
        this._read();
    }

    checkLateJobsToNotify() {
        const alreadyClosed = (jobId: number) =>
            localStorage.getItem(`jobLateClosed_${this.user.id}_${jobId}`);

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const lateJobs = (this.data.data || [])
            .filter(j => {
                if (alreadyClosed(j.jobId)) return false;
                if (!j.jobMandatoryDate) return false;

                const mandatoryDate = new Date(j.jobMandatoryDate);
                mandatoryDate.setHours(0, 0, 0, 0);

                const warningDate = new Date(mandatoryDate);
                warningDate.setDate(mandatoryDate.getDate() - 5);

                return today >= warningDate;
            });

        const uniqueJobsMap = new Map<number, any>();
        lateJobs.forEach(job => {
            if (!uniqueJobsMap.has(job.jobId)) {
                uniqueJobsMap.set(job.jobId, job);
            }
        });

        this.lateJobsToNotify = Array.from(uniqueJobsMap.values());
    }

    onLateNotificationClosed(jobId: number) {
        this.lateJobsToNotify = this.lateJobsToNotify.filter(j => j.jobId !== jobId);
    }

    exportToExcel(): void {
        const options = this.getExportOptions();
        const workbook = new Workbook(options);
        workbook.toDataURL().then((dataURL) => {
            saveAs(dataURL, 'attività.xlsx');
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
                            { value: 'Cliente', bold: true },
                            { value: 'Commessa', bold: true },
                            { value: 'Stato', bold: true },
                            { value: 'Data Scadenza', bold: true },
                            { value: 'Riferimento', bold: true },
                            { value: 'Note', bold: true },
                            { value: 'Operatore', bold: true },
                            { value: 'Inserito Da', bold: true }
                        ]
                    },
                    ...this.data.data.map((item: any) => ({
                        cells: [
                            { value: item.customer },
                            { value: item.jobCode },
                            { value: item.status },
                            { value: item.expirationDate, format: 'dd/MM/yyyy' },
                            { value: item.jobReference },
                            { value: item.shortDescription },
                            { value: item.referentName },
                            { value: item.lastOperator }
                        ]
                    }))
                ]
            }]
        };
    }
}
