import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { Job, JobStatus } from '../services/jobs/models';
import { tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { listEnum, markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';

@Component({
    selector: 'app-job-modal',
    templateUrl: 'job-modal.component.html'
})
export class JobModalComponent extends ModalComponent<Job> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    customers: CustomerModel[];

    constructor(
        private readonly _customersService: CustomerService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._getData();
    }

    onDateChange() {
        this.options.year = this.options.date?.getFullYear();
    }

    protected override _canClose() {
        markAsDirty(this.form);

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
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

}
