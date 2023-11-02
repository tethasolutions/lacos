import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { SupplierService } from '../services/supplier.service';
import { AddressesSupplierService } from '../services/addressesSupplier.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { SupplierModel } from '../shared/models/supplier.model';
import { AddressSupplierModalComponent } from '../address-modal/address-supplier-modal.component';
import { AddressSupplierModel } from '../shared/models/address-supplier.model';
import { SupplierModalComponent } from '../supplier-modal/supplier-modal.component';

@Component({
  selector: 'app-suppliers',
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.scss']
})
export class SuppliersComponent extends BaseComponent implements OnInit {

  @ViewChild('supplierModal', { static: true }) supplierModal: SupplierModalComponent;
  @ViewChild('addressSupplierModal', { static: true }) addressSupplierModal: AddressSupplierModalComponent;

  dataSuppliers: GridDataResult;
  stateGridSuppliers: State = {
      skip: 0,
      take: 10,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  supplierSelezionato = new SupplierModel();

  constructor(
      private readonly _supplierService: SupplierService,
      private readonly _addressesSupplierService: AddressesSupplierService,
      private readonly _messageBox: MessageBoxService,
  ) {
      super();
  }

  ngOnInit() {
      this._readSuppliers();
  }

  dataStateChange(state: State) {
      this.stateGridSuppliers = state;
      this._readSuppliers();
  }

  protected _readSuppliers() {
    this._subscriptions.push(
      this._supplierService.readSuppliers(this.stateGridSuppliers)
        .pipe(
            tap(e => {
              this.dataSuppliers = e;
            })
        )
        .subscribe()
    );
  }

  createSupplier() {
    const request = new SupplierModel();

    this._subscriptions.push(
        this.supplierModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._supplierService.createSupplier(request)),
                tap(e => {
                    this._messageBox.success(`Cliente ${request.name} creato`);
                }),
                tap(() => this._readSuppliers())
            )
            .subscribe()
    );
  }

  editSupplier(supplier: SupplierModel) {
    this._subscriptions.push(
      this._supplierService.getSupplier(supplier.id)
        .pipe(
            map(e => {
              return Object.assign(new SupplierModel(), e);
            }),
            switchMap(e => this.supplierModal.open(e)),
            filter(e => e),
            map(() => this.supplierModal.options),
            switchMap(e => this._supplierService.updateSupplier(e, supplier.id)),
            map(() => this.supplierModal.options),
            tap(e => this._messageBox.success(`Cliente ${e.name} aggiornato`)),
            tap(() => this._readSuppliers())
        )
      .subscribe()
    );
  }

  deleteSupplier(supplier: SupplierModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare il cliente ${supplier.name}?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._supplierService.deleteSupplier(supplier.id)
            .pipe(
              tap(e => this._messageBox.success(`Cliente ${supplier.name} cancellato con successo`)),
              tap(() => this._readSuppliers())
            )
          .subscribe()
        );
      }
    });
  }

  createAddress(supplier: SupplierModel) {
    this.supplierSelezionato = supplier;
    const request = new AddressSupplierModel();
    request.supplierId = supplier.id;
    this._subscriptions.push(
        this.addressSupplierModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._addressesSupplierService.createAddress(request)),
                tap(e => this._messageBox.success(`Indirizzo creato con successo`)),
                tap(() => this._readSuppliers())
            )
            .subscribe()
    );
  }

  editAddress(address: AddressSupplierModel, supplier: SupplierModel) {
    this.supplierSelezionato = supplier;
    this._subscriptions.push(
      this._addressesSupplierService.getAddress(address.id)
        .pipe(
            map(e => {
              return Object.assign(new AddressSupplierModel(), e);
            }),
            switchMap(e => this.addressSupplierModal.open(e)),
            filter(e => e),
            map(() => this.addressSupplierModal.options),
            switchMap(e => this._addressesSupplierService.updateAddress(e, address.id)),
            map(() => this.addressSupplierModal.options),
            tap(e => this._messageBox.success(`Indirizzo aggiornato con successo`)),
            tap(() => this._readSuppliers())
        )
      .subscribe()
    );
  }

  deleteAddress(address: AddressSupplierModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare l\'indirizzo?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._addressesSupplierService.deleteAddress(address.id)
            .pipe(
              tap(e => this._messageBox.success(`L\'indirizzo cancellato con successo`)),
              tap(() => this._readSuppliers())
            )
          .subscribe()
        );
      }
    });
  }

  setAddressAsMain(address: AddressSupplierModel) {
    this._messageBox.confirm(`Sei sicuro di voler selezionare l\'indirizzo come principale?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._addressesSupplierService.setAddressAsMain(address.id)
            .pipe(
              tap(e => this._messageBox.success(`L\'indirizzo selezionato come principale con successo`)),
              tap(() => this._readSuppliers())
            )
          .subscribe()
        );
      }
    });
  }
}
