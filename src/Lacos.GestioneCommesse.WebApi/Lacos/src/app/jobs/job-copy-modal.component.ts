import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { Job, JobCopy } from '../services/jobs/models';
import { filter, map, switchMap, tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { AddressesService } from '../services/addresses.service';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { AddressModel } from '../shared/models/address.model';

@Component({
    selector: 'app-job-copy-modal',
    templateUrl: 'job-copy-modal.component.html'
})
export class JobCopyModalComponent extends ModalFormComponent<JobCopy> implements OnInit {

    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;

    customers: CustomerModel[];
    addresses: AddressModel[];

    constructor(
        private readonly _customersService: CustomerService,
        messageBox: MessageBoxService,
        private readonly _addressesService: AddressesService
    ) {
        super(messageBox);
    }

    ngOnInit() {
    }

    override open(job: JobCopy) {
        if (this.customers == null) this._getData();
        const result = super.open(job);
        this.readAddresses();
        return result;
    }

    onCustomerChange(){
        this.addresses = this.customers.find(e => e.id == this.options.customerId)?.addresses ?? [];
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

    createAddress() {
        const request = new AddressModel();
        this._subscriptions.push(
            this.addressModal.open(request)
                .pipe(
                    filter(e => e),
                    tap(() => {
                        this.addNewAddress(request);
                    })
                )
                .subscribe()
        );
    }

    addNewAddress(address: AddressModel) {
        if (this.options.customerId !== null) address.customerId = this.options.customerId;
        this._subscriptions.push(
            this._addressesService.createAddress(address)
                .pipe(
                    tap(e => {
                        this.readAddresses();
                        this.options.addressId = e.id;
                        this._messageBox.success(`Indirizzo creato con successo`);
                    })
                )
                .subscribe()
        );
    }

    readAddresses() {
            this._subscriptions.push(
                this._addressesService.getCustomerAddresses(this.options.customerId)
                    .pipe(
                        map(e => {
                            this.addresses = e;
                        }),
                        tap(() => { })
                    )
                    .subscribe()
            );
    }
}
