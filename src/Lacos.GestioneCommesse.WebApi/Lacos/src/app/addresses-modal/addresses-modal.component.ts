import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModel } from '../shared/models/customer.model';
import { BaseComponent } from '../shared/base.component';
import { Subject } from 'rxjs';
import { AddressModel } from '../shared/models/address.model';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { AddressesService } from '../services/addresses.service';
import { CustomerService } from '../services/customer.service';
import { SupplierService } from '../services/supplier.service';
import { SupplierModel } from '../shared/models/supplier.model';
import { Job } from '../services/jobs/models';
import { Activity } from '../services/activities/models';

@Component({
    selector: 'app-addresses-modal',
    templateUrl: './addresses-modal.component.html',
    styleUrls: ['./addresses-modal.component.scss']
})

export class AddressesModalComponent extends BaseComponent {

    opened = false;

    private _closeSubject: Subject<boolean>;
    private _oveflow: string;

    @Input() customer = new CustomerModel();
    @Input() supplier = new SupplierModel();
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _addressesService: AddressesService,
        private readonly _customerService: CustomerService,
        private readonly _supplierService: SupplierService
    ) {
        super();
    }

    open() {
        if (!this._closeSubject) {
            this._closeSubject = new Subject<boolean>();
        }

        this._oveflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';

        this.opened = true;

        return this._closeSubject.asObservable();
    }

    dismiss() {
        this._closeSubject.next(false);
        this._closeSubject.complete();
        this._closeSubject = null;
        this.opened = false;
        document.body.style.overflow = this._oveflow;
    }

    createAddress() {
        const request = new AddressModel();
        if (this.customer.id === null) {
            this._subscriptions.push(
                this.addressModal.open(request)
                    .pipe(
                        filter(e => e),
                        tap(e => {
                            this.customer.addresses.push(request);
                            if (request.isMainAddress) {
                                this.customer.mainAddress = request;
                                this.customer.addresses.forEach((item: AddressModel) => {
                                    item.isMainAddress = item.tempId === request.tempId;
                                });
                            }
                            this._messageBox.success(`Indirizzo creato con successo`);
                        })
                    )
                    .subscribe()
            );
        } else {
            request.customerId = this.customer.id;
            this._subscriptions.push(
                this.addressModal.open(request)
                    .pipe(
                        filter(e => e),
                        switchMap(() => this._addressesService.createAddress(request)),
                        tap(e => this._messageBox.success(`Indirizzo creato con successo`)),
                        tap(() => this.readAddresses())
                    )
                    .subscribe()
            );
        }
        if (this.supplier.id === null) {
            this._subscriptions.push(
                this.addressModal.open(request)
                    .pipe(
                        filter(e => e),
                        tap(e => {
                            this.supplier.addresses.push(request);
                            if (request.isMainAddress) {
                                this.supplier.mainAddress = request;
                                this.supplier.addresses.forEach((item: AddressModel) => {
                                    item.isMainAddress = item.tempId === request.tempId;
                                });
                            }
                            this._messageBox.success(`Indirizzo creato con successo`);
                        })
                    )
                    .subscribe()
            );
        } else {
            request.supplierId = this.supplier.id;
            this._subscriptions.push(
                this.addressModal.open(request)
                    .pipe(
                        filter(e => e),
                        switchMap(() => this._addressesService.createAddress(request)),
                        tap(e => this._messageBox.success(`Indirizzo creato con successo`)),
                        tap(() => this.readAddresses())
                    )
                    .subscribe()
            );
        }

    }

    editAddress(address: AddressModel) {
        if (this.customer.id != null) {
            const request: AddressModel = Object.assign(new AddressModel(), JSON.parse(JSON.stringify(address)));
            this._subscriptions.push(
                this.addressModal.open(request)
                    .pipe(
                        filter(e => e),
                        tap(e => {
                            const indexAddressToEdit = this.customer.addresses.findIndex(x => x.tempId === request.tempId);
                            if (indexAddressToEdit >= 0) {
                                this.customer.addresses[indexAddressToEdit] = request;
                                if (request.isMainAddress) {
                                    // this.customer.mainAddress = request;
                                    this.customer.addresses.forEach((item: AddressModel) => {
                                        item.isMainAddress = item.tempId === request.tempId;
                                    });
                                }
                                this._messageBox.success(`Indirizzo modificato con successo`);
                            }
                        })
                    )
                    .subscribe()
            );
        } else if (this.supplier.id != null) {
            const request: AddressModel = Object.assign(new AddressModel(), JSON.parse(JSON.stringify(address)));
            this._subscriptions.push(
                this.addressModal.open(request)
                    .pipe(
                        filter(e => e),
                        tap(e => {
                            const indexAddressToEdit = this.supplier.addresses.findIndex(x => x.tempId === request.tempId);
                            if (indexAddressToEdit >= 0) {
                                this.supplier.addresses[indexAddressToEdit] = request;
                                if (request.isMainAddress) {
                                    this.supplier.mainAddress = request;
                                    this.supplier.addresses.forEach((item: AddressModel) => {
                                        item.isMainAddress = item.tempId === request.tempId;
                                    });
                                }
                                this._messageBox.success(`Indirizzo modificato con successo`);
                            }
                        })
                    )
                    .subscribe()
            );
        } else {
            this._subscriptions.push(
                this._addressesService.getAddress(address.id)
                    .pipe(
                        map(e => {
                            return Object.assign(new AddressModel(), e);
                        }),
                        switchMap(e => this.addressModal.open(e)),
                        filter(e => e),
                        map(() => this.addressModal.options),
                        switchMap(e => this._addressesService.updateAddress(e, address.id)),
                        map(() => this.addressModal.options),
                        tap(e => this._messageBox.success(`Indirizzo aggiornato con successo`)),
                        tap(() => this.readAddresses())
                    )
                    .subscribe()
            );
        }

    }

    deleteAddress(address: AddressModel) {
        if (this.customer.id === null) {
            this._messageBox.confirm(`Sei sicuro di voler cancellare l\'indirizzo?`, 'Conferma l\'azione').subscribe(result => {
                if (result == true) {
                    if (address.isMainAddress) { this.customer.mainAddress = null; }
                    this.customer.addresses = this.customer.addresses.filter(x => x.tempId !== address.tempId);
                    this._messageBox.success(`L\'indirizzo cancellato con successo`);
                }
            });
        } else if (this.supplier.id === null) {
            this._messageBox.confirm(`Sei sicuro di voler cancellare l\'indirizzo?`, 'Conferma l\'azione').subscribe(result => {
                if (result == true) {
                    if (address.isMainAddress) { this.supplier.mainAddress = null; }
                    this.supplier.addresses = this.supplier.addresses.filter(x => x.tempId !== address.tempId);
                    this._messageBox.success(`L\'indirizzo cancellato con successo`);
                }
            });
        } else {
            this._messageBox.confirm(`Sei sicuro di voler cancellare l\'indirizzo?`, 'Conferma l\'azione').subscribe(result => {
                if (result == true) {
                    this._subscriptions.push(
                        this._addressesService.deleteAddress(address.id)
                            .pipe(
                                tap(e => this._messageBox.success(`L\'indirizzo cancellato con successo`)),
                                tap(() => this.readAddresses())
                            )
                            .subscribe()
                    );
                }
            });
        }
    }

    setAddressAsMain(address: AddressModel) {
        this._messageBox.confirm(`Sei sicuro di voler selezionare l\'indirizzo come principale?`, 'Conferma l\'azione').subscribe(result => {
            if (result == true) {
                if (this.customer.id === null) {
                    this.customer.addresses.forEach((item: AddressModel) => {
                        item.isMainAddress = item.tempId === address.tempId;
                    });
                } else if (this.supplier.id === null) {
                    this.supplier.addresses.forEach((item: AddressModel) => {
                        item.isMainAddress = item.tempId === address.tempId;
                    });
                } else {
                    this._subscriptions.push(
                        this._addressesService.setAddressAsMain(address.id)
                            .pipe(
                                tap(e => this._messageBox.success(`L\'indirizzo selezionato come principale con successo`)),
                                tap(() => this.readAddresses())
                            )
                            .subscribe()
                    );
                }
            }
        });
    }

    readAddresses() {
        this._subscriptions.push(
            this._customerService.getCustomer(this.customer.id)
                .pipe(
                    map(e => {
                        const result = Object.assign(new CustomerModel(), e);
                        this.customer.addresses = result.addresses;
                    }),
                    tap(() => { })
                )
                .subscribe()
        );
        this._subscriptions.push(
            this._supplierService.getSupplier(this.supplier.id)
                .pipe(
                    map(e => {
                        const result = Object.assign(new SupplierModel(), e);
                        this.supplier.addresses = result.addresses;
                    }),
                    tap(() => { })
                )
                .subscribe()
        );
    }

    openMap(address: AddressModel) {
        const addressForMap = encodeURIComponent(address.fullAddressForDistance);
        const url = `https://www.openstreetmap.org/search?query=${addressForMap}`;
        window.open(url, '_blank');
    }

    openGoogleMap(address: AddressModel) {
        const addressForMap = encodeURIComponent(address.fullAddressForDistance);
        const url = `https://www.google.it/maps/place/${addressForMap.replace(/ /g, '+')}`;
        window.open(url, '_blank');
    }
}
