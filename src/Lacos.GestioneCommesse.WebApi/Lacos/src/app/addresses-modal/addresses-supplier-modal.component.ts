import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { SupplierModel } from '../shared/models/supplier.model';
import { BaseComponent } from '../shared/base.component';
import { Subject } from 'rxjs';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { SupplierService } from '../services/supplier.service';
import { AddressSupplierModalComponent } from '../address-modal/address-supplier-modal.component';
import { AddressSupplierModel } from '../shared/models/address-supplier.model';
import { AddressesSupplierService } from '../services/addressesSupplier.service';

@Component({
  selector: 'app-addresses-supplier-modal',
  templateUrl: './addresses-supplier-modal.component.html',
  styleUrls: ['./addresses-supplier-modal.component.scss']
})

export class AddressesSupplierModalComponent extends BaseComponent {

  opened = false;

  private _closeSubject: Subject<boolean>;
  private _oveflow: string;

  @Input() supplier = new SupplierModel();
  @ViewChild('addressSupplierModal', { static: true }) addressModal: AddressSupplierModalComponent;

  constructor(
      private readonly _messageBox: MessageBoxService,
      private readonly _addressesService: AddressesSupplierService,
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
    const request = new AddressSupplierModel();
    if (this.supplier.id === null) {
        this._subscriptions.push(
            this.addressModal.open(request)
                .pipe(
                    filter(e => e),
                    tap(e => {
                        this.supplier.addresses.push(request);
                        if (request.isMainAddress) {
                            this.supplier.mainAddress = request;
                            this.supplier.addresses.forEach((item: AddressSupplierModel) => {
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

  editAddress(address: AddressSupplierModel) {
    if (this.supplier.id === null) {
        const request: AddressSupplierModel = Object.assign(new AddressSupplierModel(), JSON.parse(JSON.stringify(address)));
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
                                this.supplier.addresses.forEach((item: AddressSupplierModel) => {
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
                    return Object.assign(new AddressSupplierModel(), e);
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

  deleteAddress(address: AddressSupplierModel) {
    if (this.supplier.id === null) {
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

  setAddressAsMain(address: AddressSupplierModel) {
    this._messageBox.confirm(`Sei sicuro di voler selezionare l\'indirizzo come principale?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        if (this.supplier.id === null){
            this.supplier.addresses.forEach((item: AddressSupplierModel) => {
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
      this._supplierService.getSupplier(this.supplier.id)
        .pipe(
            map(e => {
              const result = Object.assign(new SupplierModel(), e);
              this.supplier.addresses = result.addresses;
            }),
            tap(() => {})
        )
      .subscribe()
    );
  }
}
