import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Activity, ActivityStatus, IActivityReadModel, activityStatusNames } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivityModalComponent, ActivityModalOptions } from './activity-modal.component';
import { OperatorModel } from '../shared/models/operator.model';
import { User } from '../services/security/models';
import { CustomerService } from '../services/customer.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { CustomerModel } from '../shared/models/customer.model';
import { JobsAttachmentsModalComponent } from '../jobs/jobs-attachments-modal.component';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-activities-from-product',
    templateUrl: 'activities-from-product.component.html'
})
export class ActivitiesFromProductComponent extends BaseComponent implements OnInit {

    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('jobsAttachmentsModal', { static: true }) jobsAttachmentsModal: JobsAttachmentsModalComponent;

    productCode: string;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'startDate', dir: 'asc' }, { field: 'expirationDate', dir: 'asc' }]
    };

    public productSearchField: string;

    private cellArgs: CellClickEvent;
    user: User;
    currentOperator: OperatorModel;

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _service: ActivitiesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _customerService: CustomerService,
        private readonly route: ActivatedRoute
    ) {
        super();
    }

    ngOnInit() {
        this.productCode = this.route.snapshot.paramMap.get('productCode') ?? '';
        if (this.productCode) {
            this.productSearchField = this.productCode.toString();
            this._search();
        }
    }

    filterProduct() {
        this._search();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._search();
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

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    readonly rowCallback = (context: RowClassArgs) => {
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

    protected _search() {
        this._subscriptions.push(
            this._service.activitiesFromProduct(this.gridState, this.productSearchField)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    private _afterActivityUpdated() {
        this._messageBox.success('Attività aggiornata.')

        this._search();
    }

    private _afterRemoved(activity: IActivityReadModel) {
        const text = `Attività ${activity.number} rimossa.`;

        this._messageBox.success(text);

        this._search();
    }

}
