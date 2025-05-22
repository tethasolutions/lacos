import { Component, input, Input } from '@angular/core';
import { GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { ActivityStatus, IActivityReadModel } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivatedRoute } from '@angular/router';
import { Subject, tap } from 'rxjs';
import { State } from '@progress/kendo-data-query';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { IPurchaseOrderReadModel, PurchaseOrderStatus } from '../services/purchase-orders/models';
import { DependencyModel } from '../shared/models/dependency.models';

@Component({
    selector: 'app-dependencies-modal',
    templateUrl: 'dependencies-modal.component.html'
})
export class DependenciesModalComponent extends BaseComponent {

    @Input() jobId: number;
    @Input() activityId: number;
    @Input() readonly: boolean = false;

    opened = false;
    private _closeSubject: Subject<boolean>;

    dataActivities: GridDataResult;
    gridActivityState: State = {
        skip: 0,
        take: 30,
        group: [],
        sort: [{ field: 'startDate', dir: 'asc' }, { field: 'expirationDate', dir: 'asc' }]
    };

    dataPurchaseOrders: GridDataResult;
    gridPurchaseOrderState: State = {
        skip: 0,
        take: 30,
        group: [],
        sort: [{ field: 'date', dir: 'desc' }, { field: 'code', dir: 'desc' }]
    };

    selectedActivityIds: number[] = [];
    selectedPurchaseOrderIds: number[] = [];

    constructor(
        private readonly _activitiesService: ActivitiesService,
        private readonly _purchaseOrdersService: PurchaseOrdersService,
        private readonly _messageBoxService: MessageBoxService
    ) {
        super();
    }

    open() {
        if (!this._closeSubject) {
            this._closeSubject = new Subject<boolean>();
        }
        this.opened = true;
        this._loadDependencies();

        return this._closeSubject.asObservable();
    }

    dismiss() {
        this._closeSubject.next(false);
        this._closeSubject.complete();
        this._closeSubject = null;
        this.opened = false;
    }

    private _loadDependencies() {
        this._subscriptions.push(
            this._activitiesService.readDependencies(this.activityId)
                .pipe(
                    tap((dependencies: DependencyModel) => {
                        this.selectedActivityIds = dependencies.activityDependenciesId || [];
                        this.selectedPurchaseOrderIds = dependencies.purchaseOrderDependenciesId || [];
                        this._readActivities();
                        this._readPurchaseOrders();
                    })
                )
                .subscribe()
        );
    }

    readonly rowCallbackActivities = (context: RowClassArgs) => {
        const activity = context.dataItem as IActivityReadModel;

        switch (true) {
            case activity.status === ActivityStatus.Completed:
                return { 'activity-completed': true };
            case activity.status === ActivityStatus.Pending:
                return { 'activity-pending': true };
            case activity.status === ActivityStatus.InProgress:
                return { 'activity-in-progress': true };
            case activity.status === ActivityStatus.Ready:
                return { 'activity-ready': true };
            case activity.status != ActivityStatus.Completed && !!activity.expirationDate && new Date(activity.expirationDate).addDays(1).isPast():
                return { 'activity-expired': true };
            default:
                return {};
        }
    };

    readonly rowCallbackPurchaseOrders = (context: RowClassArgs) => {
        const purchaseOrder = context.dataItem as IPurchaseOrderReadModel;

        switch (true) {
            case purchaseOrder.status === PurchaseOrderStatus.Completed:
                return { 'purchase-order-completed': true };
            case purchaseOrder.status === PurchaseOrderStatus.Pending:
                return { 'purchase-order-pending': true };
            case purchaseOrder.status === PurchaseOrderStatus.Ordered:
                return { 'purchase-order-ordered': true };
            case purchaseOrder.status === PurchaseOrderStatus.Partial:
                return { 'purchase-order-partial': true };
            case purchaseOrder.status === PurchaseOrderStatus.Canceled:
                return { 'purchase-order-canceled': true };
            default:
                return {};
        }
    };

    dataActivitiesStateChange(state: State) {
        this.gridActivityState = state;
        this._readActivities();
    }

    protected _readActivities() {
        this._subscriptions.push(
            this._activitiesService.readJobActivities(this.gridActivityState, this.jobId)
                .pipe(
                    tap(e => this.dataActivities = e)
                )
                .subscribe()
        );
    }

    onActivitySelectionChange(event: any) {
        event.selectedRows.forEach((row: any) => {
            const id = row.dataItem.id;
            if (!this.selectedActivityIds.includes(id)) {
                this.selectedActivityIds.push(id);
            }
        });
        event.deselectedRows.forEach((row: any) => {
            const id = row.dataItem.id;
            this.selectedActivityIds = this.selectedActivityIds.filter(selectedId => selectedId !== id);
        });
    }

    dataPurchaseOrdersStateChange(state: State) {
        this.gridPurchaseOrderState = state;
        this._readPurchaseOrders();
    }

    protected _readPurchaseOrders() {
        this._subscriptions.push(
            this._purchaseOrdersService.readJobPurchaseOrders(this.gridPurchaseOrderState, this.jobId)
                .pipe(
                    tap(e => this.dataPurchaseOrders = e)
                )
                .subscribe()
        );
    }

    onPurchaseOrderSelectionChange(event: any) {
        event.selectedRows.forEach((row: any) => {
            const id = row.dataItem.id;
            if (!this.selectedPurchaseOrderIds.includes(id)) {
                this.selectedPurchaseOrderIds.push(id);
            }
        });
        event.deselectedRows.forEach((row: any) => {
            const id = row.dataItem.id;
            this.selectedPurchaseOrderIds = this.selectedPurchaseOrderIds.filter(selectedId => selectedId !== id);
        });
    }

    saveDepencencies() {
        const dependencies: DependencyModel = {
            activityDependenciesId: this.selectedActivityIds,
            purchaseOrderDependenciesId: this.selectedPurchaseOrderIds
        };

        this._subscriptions.push(
            this._activitiesService.updateDependencies(this.activityId, dependencies)
                .pipe(
                    tap(() => {
                        this._messageBoxService.success('Dipendenze associate con successo');
                        this.dismiss();
                    })
                )
                .subscribe()
        );
    }

}
