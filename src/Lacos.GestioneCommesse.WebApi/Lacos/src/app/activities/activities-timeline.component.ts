import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
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
import { Role, User } from '../services/security/models';
import { CustomerService } from '../services/customer.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { CustomerModel } from '../shared/models/customer.model';
import { StorageService } from '../services/common/storage.service';
import { JobsAttachmentsModalComponent } from '../jobs/jobs-attachments-modal.component';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';
import { DependenciesModalComponent } from '../dependencies/dependencies-modal.component';
import { Job } from '../services/jobs/models';
import { ActivityTypesService } from '../services/activityTypes.service';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { InterventionsService } from '../services/interventions/interventions.service';
import { IInterventionReadModel, Intervention, interventionStatusNames } from '../services/interventions/models';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { IPurchaseOrderReadModel, purchaseOrderStatusNames } from '../services/purchase-orders/models';
import { DependencyModel } from '../shared/models/dependency.models';
import { InterventionModalComponent } from '../interventions/intervention-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { PurchaseOrderModalComponent, PurchaseOrderModalOptions } from '../purchase-order/purchase-order-modal.component';

@Component({
    selector: 'app-activities-timeline',
    templateUrl: 'activities-timeline.component.html',
    styleUrls: ['activities-timeline.component.css']
})
export class ActivitiesTimelineComponent extends BaseComponent implements OnInit {
    [x: string]: any;

    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('jobsAttachmentsModal', { static: true }) jobsAttachmentsModal: JobsAttachmentsModalComponent;
    @ViewChild('dependenciesModal', { static: false }) dependenciesModal: DependenciesModalComponent;
    @ViewChild('interventionModal', { static: true }) interventionModal: InterventionModalComponent;
    @ViewChild('purchaseOrderModal', { static: true }) purchaseOrderModal: PurchaseOrderModalComponent;

    data: GridDataResult;
    activities: IActivityReadModel[] = [];
    selectedActivity: IActivityReadModel = null;
    dependentActivities: IActivityReadModel[] = [];
    dependentPurchaseOrders: IPurchaseOrderReadModel[] = [];
    interventions: IInterventionReadModel[] = [];
    detailLoading: boolean = false;
    readonly interventionStatusNames = interventionStatusNames;
    readonly purchaseOrderStatusNames = purchaseOrderStatusNames;
    gridState: State = {
        skip: 0,
        take: 1000,
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
        sort: [{ field: 'startDate', dir: 'asc' }, { field: 'expirationDate', dir: 'asc' }, { field: 'number', dir: 'asc' }]
    };

    private _jobId: number;
    private _typeId: number;
    private _referentId: number;
    user: User;
    currentOperator: OperatorModel;
    job: Job;
    lateJobsToNotify: any[] = [];
    lateActivitiesToNotify: IActivityReadModel[] = [];
    isOperator: boolean = true;

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _service: ActivitiesService,
        private readonly _activityTypeService: ActivityTypesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _route: ActivatedRoute,
        private readonly _user: UserService,
        private readonly _customerService: CustomerService,
        private readonly _operatorsService: OperatorsService,
        private readonly _jobsService: JobsService,
        private readonly _interventionsService: InterventionsService,
        private readonly _purchaseOrdersService: PurchaseOrdersService
    ) {
        super();
        this.data = null;
    }

    ngOnInit() {
        this._subscribeRouteParams();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
        this.isOperator = (this.user.role == Role.Operator);

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

    askRemove(activity: IActivityReadModel) {
        const text = `Sei sicuro di voler rimuovere l'attività ${activity.shortDescription}?`;

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
        const activity = new Activity(0, ActivityStatus.Pending, null, null, null, null, this._jobId, null, null, null, null, null, null, false,
            "In attesa", "In corso", "Pronto", "Completata", false, false, false, false, [], []);
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

    getTimelineItemClass(item: IActivityReadModel): { [key: string]: boolean } {
        return {
            'status-pending': item.status === ActivityStatus.Pending,
            'status-inprogress': item.status === ActivityStatus.InProgress,
            'status-ready': item.status === ActivityStatus.Ready,
            'status-completed': item.status === ActivityStatus.Completed,
            'is-expired': item.status !== ActivityStatus.Completed && item.isExpired
        };
    }

    getStatusBadgeClass(item: IActivityReadModel): { [key: string]: boolean } {
        return {
            'badge-pending': item.status === ActivityStatus.Pending,
            'badge-inprogress': item.status === ActivityStatus.InProgress,
            'badge-ready': item.status === ActivityStatus.Ready,
            'badge-completed': item.status === ActivityStatus.Completed
        };
    }

    selectActivity(item: IActivityReadModel) {
        if (this.selectedActivity?.id === item.id) {
            this.selectedActivity = null;
            this.dependentActivities = [];
            this.dependentPurchaseOrders = [];
            this.interventions = [];
            return;
        }

        this.selectedActivity = item;
        this.dependentActivities = [];
        this.dependentPurchaseOrders = [];
        this.interventions = [];
        this.detailLoading = true;

        this._loadInterventions(item);
        this._loadDependencies(item);
    }

    private _loadInterventions(item: IActivityReadModel) {
        const state: State = {
            skip: 0,
            take: 1000,
            filter: {
                filters: [{ field: 'activityId', operator: 'eq', value: item.id }],
                logic: 'and'
            },
            sort: [{ field: 'start', dir: 'desc' }]
        };

        this._subscriptions.push(
            this._interventionsService.read(state)
                .pipe(
                    tap(e => {
                        this.interventions = (e.data || []) as IInterventionReadModel[];
                        this._checkDetailLoaded();
                    })
                )
                .subscribe()
        );
    }

    private _loadDependencies(item: IActivityReadModel) {
        if (!item.hasDependencies) {
            this.detailLoading = false;
            return;
        }

        this._subscriptions.push(
            this._service.readDependencies(item.id)
                .pipe(
                    tap(deps => this._loadDependencyDetails(item, deps))
                )
                .subscribe()
        );
    }

    private _loadDependencyDetails(item: IActivityReadModel, deps: DependencyModel) {
        if (deps.activityDependenciesId?.length > 0) {
            const state: State = {
                skip: 0,
                take: 1000,
                filter: {
                    filters: deps.activityDependenciesId.map(id => ({ field: 'id', operator: 'eq', value: id })),
                    logic: 'or'
                }
            };
            this._subscriptions.push(
                this._service.readJobActivities(state, item.jobId)
                    .pipe(
                        tap(e => {
                            this.dependentActivities = (e.data || []) as IActivityReadModel[];
                            this._checkDetailLoaded();
                        })
                    )
                    .subscribe()
            );
        }

        if (deps.purchaseOrderDependenciesId?.length > 0) {
            const state: State = {
                skip: 0,
                take: 1000,
                filter: {
                    filters: deps.purchaseOrderDependenciesId.map(id => ({ field: 'id', operator: 'eq', value: id })),
                    logic: 'or'
                }
            };
            this._subscriptions.push(
                this._purchaseOrdersService.readJobPurchaseOrders(state, item.jobId)
                    .pipe(
                        tap(e => {
                            this.dependentPurchaseOrders = (e.data || []) as IPurchaseOrderReadModel[];
                            this._checkDetailLoaded();
                        })
                    )
                    .subscribe()
            );
        }

        if (!deps.activityDependenciesId?.length && !deps.purchaseOrderDependenciesId?.length) {
            this._checkDetailLoaded();
        }
    }

    private _checkDetailLoaded() {
        this.detailLoading = false;
    }

    downloadReport(interventionId: number) {
        const user = this._user.getUser();
        window.open(`${ApiUrls.baseApiUrl}/interventions/download-report/${interventionId}?access_token=${user.accessToken}`, '_blank');
    }

    sendReport(interventionId: number, customerEmail: string) {
        this._subscriptions.push(
            this._messageBox.confirm('Vuoi inviare il rapportino alla mail del cliente (' + customerEmail + ')?', 'Invio rapportino')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._interventionsService.sendReport(interventionId, customerEmail)),
                    tap(() => this._messageBox.success('Rapportino inviato.'))
                )
                .subscribe()
        );
    }

    editIntervention(intervention: IInterventionReadModel) {
        this._subscriptions.push(
            this._interventionsService.get(intervention.id)
                .pipe(
                    switchMap(e => this.interventionModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._interventionsService.update(this.interventionModal.options)),
                    tap(() => {
                        this._messageBox.success('Intervento aggiornato.');
                        if (this.selectedActivity) {
                            this._loadInterventions(this.selectedActivity);
                        }
                    })
                )
                .subscribe()
        );
    }

    askRemoveIntervention(intervention: IInterventionReadModel) {
        if (!intervention.canBeRemoved) {
            return;
        }

        this._subscriptions.push(
            this._messageBox.confirm('Sei sicuro di voler rimuovere l\'intervento selezionato?', 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._interventionsService.delete(intervention.id)),
                    tap(() => {
                        this._messageBox.success('Intervento rimosso.');
                        if (this.selectedActivity) {
                            this._loadInterventions(this.selectedActivity);
                        }
                    })
                )
                .subscribe()
        );
    }

    editPurchaseOrder(purchaseOrder: IPurchaseOrderReadModel) {
        this._subscriptions.push(
            this._purchaseOrdersService.get(purchaseOrder.id)
                .pipe(
                    map(e => new PurchaseOrderModalOptions(e)),
                    switchMap(e => this.purchaseOrderModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._purchaseOrdersService.update(this.purchaseOrderModal.options.purchaseOrder)),
                    tap(() => {
                        this._messageBox.success('Ordine aggiornato.');
                        if (this.selectedActivity) {
                            this._loadDependencies(this.selectedActivity);
                        }
                    })
                )
                .subscribe()
        );
    }

    askRemovePurchaseOrder(purchaseOrder: IPurchaseOrderReadModel) {
        const text = `Sei sicuro di voler rimuovere l'ordine selezionato?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._purchaseOrdersService.delete(purchaseOrder.id)),
                    tap(() => {
                        this._messageBox.success('Ordine rimosso.');
                        if (this.selectedActivity) {
                            this._loadDependencies(this.selectedActivity);
                        }
                    })
                )
                .subscribe()
        );
    }

    protected _read() {
        this._subscriptions.push(
            this._service.readExternals(this.gridState)
                .pipe(
                    tap(e => {
                        this.data = e;
                        this.activities = (e.data || []) as IActivityReadModel[];
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

        if (this._typeId != null) {
            this._activityTypeService.getActivityTypeDetail(this._typeId)
                .pipe(
                    map(e => {
                        const activityType = Object.assign(new ActivityTypeModel(), e);
                        this.console.log(activityType);
                    })
                ).subscribe();
        }

        this._read();
    }

    checkLateJobsToNotify() {
        const jobAlreadyClosed = (jobId: number) =>
            localStorage.getItem(`jobLateClosed_${this.user.id}_${jobId}`);

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const lateJobs = (this.data.data || [])
            .filter(j => {
                if (jobAlreadyClosed(j.jobId)) return false;
                if (!j.jobMandatoryDate) return false;
                if (j.jobReferentId != this.currentOperator.id &&
                    j.jobCreatorUserId != this.user.id) return false;

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

        const actAlreadyClosed = (id: number) =>
            localStorage.getItem(`activityLateClosed_${this.user.id}_${id}`);

        this.lateActivitiesToNotify = (this.data.data || [])
            .filter(a => {
                if (actAlreadyClosed(a.id)) return false;
                if (!a.isMandatoryExpiration) return false;
                if ((
                    a.lastOperator != this.user.userName &&
                    a.referentId != this.currentOperator.id
                ) ||
                    (
                        this.currentOperator.activityTypes.length > 0
                        && !this.currentOperator.activityTypes.contains(a.typeId)
                    )
                ) return false;
                return a.isExpired;
            });
    }

    onLateJobNotificationClosed(jobId: number) {
        this.lateJobsToNotify = this.lateJobsToNotify.filter(j => j.jobId !== jobId);
    }

    onLateActivityNotificationClosed(id: number) {
        this.lateActivitiesToNotify = this.lateActivitiesToNotify.filter(a => a.id !== id);
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
                title: 'Attività',
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
