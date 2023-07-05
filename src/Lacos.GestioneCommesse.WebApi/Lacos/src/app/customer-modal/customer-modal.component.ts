import { Component, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { CustomerModel } from '../shared/models/customer.model';
import { AddressModel } from '../shared/models/address.model';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { AddressesModalComponent } from '../addresses-modal/addresses-modal.component';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { CustomerService } from '../services/customer.service';
import { AddressesService } from '../services/addresses.service';

@Component({
  selector: 'app-customer-modal',
  templateUrl: './customer-modal.component.html',
  styleUrls: ['./customer-modal.component.scss']
})

export class CustomerModalComponent extends ModalComponent<CustomerModel> {
  
    @ViewChild('form') form: NgForm;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
    @ViewChild('addressesModal', { static: true }) addressesModal: AddressesModalComponent;

    readonly role = Role;

    get isAddressInValidationError(): boolean {
        if (this.form == undefined) { return false; }
        return this.form.submitted && !this.form.controls['address'].valid;
    }

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _customerService: CustomerService,
        private readonly _addressesService: AddressesService
    ) {
        super();
    }

    protected _canClose() {
        markAsDirty(this.form);

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi');
        }

        return this.form.valid;
    }

    mainAddressChanged(address: AddressModel) {
        if (address === undefined) { return; }
        this.options.addresses.forEach((item: AddressModel) => {
            item.isMainAddress = item.tempId === address.tempId;
        });
    }

    addNewAddress(address: AddressModel) {
        if (this.options.id == null) {
            this.options.addresses.push(address);
            if (address.isMainAddress) {
                this.options.mainAddress = address;
                this.options.addresses.forEach((item: AddressModel) => {
                    item.isMainAddress = item.tempId === address.tempId;
                });
            }
        } else {
            this._subscriptions.push(
                this._addressesService.createAddress(address)
                    .pipe(
                        map(e => e),
                        tap(e => this._messageBox.success(`Indirizzo creato con successo`)),
                        tap(() => this.readAddresses())
                    )
                    .subscribe()
            );
        }
    }

    readAddresses() {
        this._subscriptions.push(
          this._customerService.getCustomer(this.options.id)
            .pipe(
                map(e => {
                  const result = Object.assign(new CustomerModel(), e);
                  this.options.addresses = result.addresses;
                }),
                tap(() => {})
            )
          .subscribe()
        );
      }

    createAddress() {
        const request = new AddressModel();
        request.contactId = this.options.id;
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

    editAddresses() {
        this._subscriptions.push(
            this.addressesModal.open()
                .pipe(
                    filter(e => e),
                    tap(() => {
                        console.log('closed edit addresses')
                    })
                )
                .subscribe()
        );
    }
}
