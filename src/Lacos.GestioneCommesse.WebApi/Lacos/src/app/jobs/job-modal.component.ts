import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { Job } from '../services/jobs/models';
import { filter, switchMap, tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { refreshUserData } from '../services/security/security.service';

@Component({
    selector: 'app-job-modal',
    templateUrl: 'job-modal.component.html'
})
export class JobModalComponent extends ModalComponent<Job> implements OnInit {

    @ViewChild('customerModal', { static: true }) 
    customerModal: CustomerModalComponent;
    
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
        this.form.markAsDirty();

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
                        this._messageBox.success(`Cliente ${request.name} creato`);
                    }),
                    tap(() => this._getData())
                )
                .subscribe()
        );
      }
}
