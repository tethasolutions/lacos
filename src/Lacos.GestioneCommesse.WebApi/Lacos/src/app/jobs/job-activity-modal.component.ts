import { Component, OnInit, ViewChild } from '@angular/core';
import { Activity } from '../services/activities/models';
import { ModalComponent } from '../shared/modal.component';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { tap } from 'rxjs';
import { ActivityTypesService } from '../services/activityTypes.service';
import { CustomerService } from '../services/customer.service';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModel } from '../shared/models/customer.model';

@Component({
    selector: 'app-job-activity-modal',
    templateUrl: 'job-activity-modal.component.html'
})
export class JobActivityModalComponent extends ModalComponent<JobActivityModalOptions> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    activityTypes: ActivityTypeModel[];
    customer: CustomerModel;

    constructor(
        private readonly _activityTypesService: ActivityTypesService,
        private readonly _customersService: CustomerService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._getActivityTypes();
    }

    override open(options: JobActivityModalOptions) {
        const result = super.open(options);

        this._getData();

        return result;
    }

    protected override _canClose() {
        markAsDirty(this.form);

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

    private _getData() {
        this._subscriptions.push(
            this._customersService.getCustomer(this.options.customerId)
                .pipe(
                    tap(e => this.customer = e)
                )
                .subscribe()
        );
    }

}

export class JobActivityModalOptions {

    constructor(
        readonly customerId: number,
        readonly activity: Activity
    ) {
    }

}
