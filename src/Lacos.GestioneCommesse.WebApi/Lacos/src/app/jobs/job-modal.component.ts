import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { Job, JobStatus } from '../services/jobs/models';
import { filter, map, switchMap, tap } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { refreshUserData } from '../services/security/security.service';
import { AddressesService } from '../services/addresses.service';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { AddressModel } from '../shared/models/address.model';
import { WindowState } from '@progress/kendo-angular-dialog';
import { JobsService } from '../services/jobs/jobs.service';
import { listEnum } from '../services/common/functions';

@Component({
    selector: 'app-job-modal',
    templateUrl: 'job-modal.component.html'
})
export class JobModalComponent extends ModalComponent<Job> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;

    customers: CustomerModel[];
    addresses: AddressModel[];
    public windowState: WindowState = "default";

    readonly states = listEnum<JobStatus>(JobStatus);

    constructor(
        private readonly _customersService: CustomerService,
        private readonly _service: JobsService,
        private readonly _messageBox: MessageBoxService,
        private readonly _addressesService: AddressesService
    ) {
        super();
    }

    ngOnInit() {
        this._getData();
    }

    onDateChange() {
        this.options.year = this.options.date?.getFullYear();
    }

    override open(job: Job) {
        const result = super.open(job);
        this.readAddresses();
        return result;
    }

    onCustomerChange(){
        const selectedCustomer = this.customers.find(e => e.id == this.options.customerId);
        this.addresses = selectedCustomer?.addresses ?? [];
        if (selectedCustomer != undefined) {
        const selectedAddress: AddressModel = selectedCustomer.addresses.find(x => x.isMainAddress == true);
            if (selectedAddress != undefined) {
            this.options.addressId = selectedAddress.id;
            }
        }
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
    
    deleteJob() {
        this._messageBox.confirm(`Sei sicuro di voler cancellare la commessa?`, 'Conferma l\'azione').subscribe(result => {
          if (result == true) {
            this._subscriptions.push(
              this._service.delete(this.options.id)
                .pipe(
                  tap(e => this._messageBox.success(`Commessa cancellata con successo`)),
                  tap(() => this.dismiss())
                )
                .subscribe()
            );
          }
        });
      }

}
