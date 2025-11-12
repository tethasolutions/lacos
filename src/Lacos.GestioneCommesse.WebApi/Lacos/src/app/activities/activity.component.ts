import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseComponent } from '../shared/base.component';
import { filter, switchMap, tap } from 'rxjs';
import { ActivityDetail } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { NgForm } from '@angular/forms';
import { ActivityModalComponent, ActivityModalOptions } from './activity-modal.component';
import { ActivityProductModalComponent, ActivityProductModalOptions } from './activity-product-modal.component';
import { ActivityProduct, IActivityProductReadModel } from '../services/activity-products/models';
import { ActivityProductsService } from '../services/activity-products/activity-products.service';
import { ActivityProductsComponent } from './activity-products.component';
import { InterventionModalComponent } from '../interventions/intervention-modal.component';
import { InterventionsService } from '../services/interventions/interventions.service';
import { Intervention, InterventionStatus } from '../services/interventions/models';
import { InterventionsCalendarComponent } from '../interventions/interventions-calendar.component';
import { InterventionsGridComponent } from '../interventions/interventions-grid.component';
import { ApiUrls } from '../services/common/api-urls';
import { JobModalComponent } from '../jobs/job-modal.component';
import { JobsService } from '../services/jobs/jobs.service';
import { DependenciesModalComponent } from '../dependencies/dependencies-modal.component';

@Component({
    selector: 'app-activity',
    templateUrl: 'activity.component.html'
})
export class ActivityComponent extends BaseComponent implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;

    @ViewChild('jobModal', { static: true }) jobModal: JobModalComponent;
    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('activityProductModal', { static: true }) activityProductModal: ActivityProductModalComponent;
    @ViewChild('activityProducts', { static: false }) activityProducts: ActivityProductsComponent;
    @ViewChild('interventionsCalendar', { static: false }) interventionsCalendar: InterventionsCalendarComponent;
    @ViewChild('interventionsGrid', { static: false }) interventionsGrid: InterventionsGridComponent;
    @ViewChild('interventionModal', { static: true }) interventionModal: InterventionModalComponent;
    @ViewChild('dependenciesModal', { static: false }) dependenciesModal: DependenciesModalComponent;

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;
    activity: ActivityDetail;

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _service: ActivitiesService,
        private readonly _jobService: JobsService,
        private readonly _messageBox: MessageBoxService,
        private readonly _activityProductsService: ActivityProductsService,
        private readonly _interventionsService: InterventionsService
    ) {
        super();
    }

    ngOnInit() {
        this._subscribeRouteParams();
    }

    edit() {
        const activity = this.activity.asActivity();
        const options = new ActivityModalOptions(activity);

        this._subscriptions.push(
            this.activityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.update(activity)),
                    tap(() => this._afterActivityUpdated())
                )
                .subscribe()
        );
    }

    editJob(jobId: number) {
        this._subscriptions.push(
            this._jobService.get(jobId)
                .pipe(
                    switchMap(e => this.jobModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._jobService.update(this.jobModal.options)),
                    tap(e => location.reload())
                )
                .subscribe()
        );
    }

    createActivityProduct() {
        const product = new ActivityProduct(0, this.activity.id, null, null, null, null);
        const options = new ActivityProductModalOptions(product);

        this._subscriptions.push(
            this.activityProductModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._activityProductsService.create(product)),
                    tap(() => this._afterActivityProductCreated(this.activityProductModal.product.name))
                )
                .subscribe()
        );
    }

    editActivityProduct(product: IActivityProductReadModel) {
        this._subscriptions.push(
            this._activityProductsService.get(product.id)
                .pipe(
                    switchMap(e => this.activityProductModal.open(new ActivityProductModalOptions(e))),
                    filter(e => e),
                    switchMap(() => this._activityProductsService.update(this.activityProductModal.options.product)),
                    tap(() => this.activityProducts.refresh())
                )
                .subscribe()
        );
    }

    createIntervention() {
        if (this.activity.hasUnpaidAccounts) {
            this._subscriptions.push(
                this._messageBox.confirm('Attenzione: l\'attività è collegata a conti non saldati. Continuare ugualmente?')
                    .pipe(
                        filter(result => result === true),
                        tap(() => this._createIntervention())
                    )
                    .subscribe()
            );
        } else {
            this._createIntervention();
        }
    }

    private _createIntervention() {
        const now = new Date();
        const intervention = new Intervention(0, new Date(new Date(new Date().setMinutes(0)).setSeconds(0)),
            new Date(new Date(new Date().setMinutes(0)).setSeconds(0)).addHours(1), InterventionStatus.Scheduled, false,
            this.activity.informations, null, null, this.activity.id, this.activity.jobId, [], [], []);

        this._subscriptions.push(
            this.interventionModal.open(intervention)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._interventionsService.create(intervention)),
                    tap(() => this._afterInterventionCreated())
                )
                .subscribe()
        );
    }

    onInterventionsChanged() {
        this._getActivity();
    }

    openDependencies(jobId: number, activityId: number) {
        this.dependenciesModal.jobId = jobId;
        this.dependenciesModal.activityId = activityId;
        this.dependenciesModal.readonly = true;
        this.dependenciesModal.open();
    }

    assignAllCustomerProducts() {
        const text = 'Sei sicuro di voler associare all\'attività tutti i prodotti di ' + this.activity.customer + '?';

        this._subscriptions.push(
            this._messageBox.confirm(text)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.assignAllCustomerProducts(this.activity.id)),
                    tap(() => this._afterAllCustomerProductsAssigned())
                )
                .subscribe()
        );
    }

    assignAllCustomerProductsMonthlyMaint() {
        const text = 'Sei sicuro di voler associare all\'attività tutti i prodotti di ' + this.activity.customer + ' con manutenzione mensile?';

        this._subscriptions.push(
            this._messageBox.confirm(text)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.assignAllCustomerProductsMonthlyMaint(this.activity.id)),
                    tap(() => this._afterAllCustomerProductsAssigned())
                )
                .subscribe()
        );
    }

    private _subscribeRouteParams() {
        this._subscriptions.push(
            this._route.data
                .pipe(
                    tap(e => this.activity = e['activity'] as ActivityDetail)
                )
                .subscribe()
        );
    }

    private _afterActivityUpdated() {
        this._messageBox.success(`Attività aggiornata.`);

        this._subscriptions.push(
            this._service.getDetail(this.activity.id)
                .pipe(
                    tap(e => this.activity = e)
                )
                .subscribe()
        );
    }

    private _afterActivityProductCreated(name: string) {
        this._messageBox.success(`${name} associato all'attività.`);

        this.activityProducts?.refresh();
    }

    private _afterAllCustomerProductsAssigned() {
        this._messageBox.success(`Prodotti associati all'attività.`);

        this.activityProducts?.refresh();
    }

    private _afterInterventionCreated() {
        this._messageBox.success(`Intervento programmato.`);
        this._getActivity();
        this.interventionsCalendar?.refresh();
        this.interventionsGrid?.refresh();
    }

    private _getActivity() {
        this._subscriptions.push(
            this._service.getDetail(this.activity.id)
                .pipe(
                    tap(e => this.activity = e)
                )
                .subscribe()
        );
    }
}
