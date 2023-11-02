import { Component, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { SupplierModel } from '../shared/models/supplier.model';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { AddressesModalComponent } from '../addresses-modal/addresses-modal.component';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { SupplierService } from '../services/supplier.service';
import { AddressSupplierModalComponent } from '../address-modal/address-supplier-modal.component';
import { AddressSupplierModel } from '../shared/models/address-supplier.model';
import { AddressesSupplierService } from '../services/addressesSupplier.service';
import { AddressesSupplierModalComponent } from '../addresses-modal/addresses-supplier-modal.component';

@Component({
    selector: 'app-supplier-modal',
    templateUrl: './supplier-modal.component.html',
    styleUrls: ['./supplier-modal.component.scss']
})

export class SupplierModalComponent extends ModalComponent<SupplierModel> {

    @ViewChild('form') form: NgForm;
    @ViewChild('addressSupplierModal', { static: true }) addressSupplierModal: AddressSupplierModalComponent;
    @ViewChild('addressesSupplierModal', { static: true }) addressesSupplierModal: AddressesSupplierModalComponent;

    readonly role = Role;
    
    get isAddressInValidationError(): boolean {
        if (this.form == undefined) { return false; }
        return this.form.submitted && !this.form.controls['address'].valid;
    }

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _supplierService: SupplierService,
        private readonly _addressesService: AddressesSupplierService
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

    mainAddressChanged(address: AddressSupplierModel) {
        if (address === undefined) { return; }
        this.options.addresses.forEach((item: AddressSupplierModel) => {
            item.isMainAddress = item.tempId === address.tempId;
        });
    }

    addNewAddress(address: AddressSupplierModel) {
        if (this.options.id == null) {
            this.options.addresses.push(address);
            if (address.isMainAddress) {
                this.options.mainAddress = address;
                this.options.addresses.forEach((item: AddressSupplierModel) => {
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
            this._supplierService.getSupplier(this.options.id)
                .pipe(
                    map(e => {
                        const result = Object.assign(new SupplierModel(), e);
                        this.options.addresses = result.addresses;
                    }),
                    tap(() => { })
                )
                .subscribe()
        );
    }

    createAddress() {
        const request = new AddressSupplierModel();
        request.supplierId = this.options.id;
        this._subscriptions.push(
            this.addressSupplierModal.open(request)
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
            this.addressesSupplierModal.open()
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
