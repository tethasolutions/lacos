import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { CustomerService } from '../services/customer.service';
import { AddressesService } from '../services/addresses.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { CustomerModel } from '../shared/models/customer.model';
import { AddressModel } from '../shared/models/address.model';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { Router, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.scss']
})
export class CustomersComponent extends BaseComponent implements OnInit {

  @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
  @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;

  dataCustomers: GridDataResult;
  stateGridCustomers: State = {
      skip: 0,
      take: 10,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  customerSelezionato = new CustomerModel();

  anagraficaType: string;

  constructor(
      private readonly _customerService: CustomerService,
      private readonly _addressesService: AddressesService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      if (this._router.url === '/customers') { this.anagraficaType = 'customers'; }
      if (this._router.url === '/providers') { this.anagraficaType = 'providers'; }
      this._readCustomers();
  }

  dataStateChange(state: State) {
      this.stateGridCustomers = state;
      this._readCustomers();
  }

  protected _readCustomers() {
    this._subscriptions.push(
      this._customerService.readCustomers(this.stateGridCustomers, this.anagraficaType)
        .pipe(
            tap(e => {
              this.dataCustomers = e;
            })
        )
        .subscribe()
    );
  }

  createCustomer() {
    const request = new CustomerModel();

    this._subscriptions.push(
        this.customerModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._customerService.createCustomer(request)),
                tap(e => {
                  if (this.anagraficaType == 'customers') { 
                    this._messageBox.success(`Cliente ${request.name} creato`);
                  }
                  if (this.anagraficaType == 'providers') { 
                    this._messageBox.success(`Fornitore ${request.name} creato`);
                  }
                }),
                tap(() => this._readCustomers())
            )
            .subscribe()
    );
  }

  editCustomer(customer: CustomerModel) {
    this._subscriptions.push(
      this._customerService.getCustomer(customer.id)
        .pipe(
            map(e => {
              return Object.assign(new CustomerModel(), e);
            }),
            switchMap(e => this.customerModal.open(e)),
            filter(e => e),
            map(() => this.customerModal.options),
            switchMap(e => this._customerService.updateCustomer(e, customer.id)),
            map(() => this.customerModal.options),
            tap(e => this._messageBox.success(`Cliente ${e.name} aggiornato`)),
            tap(() => this._readCustomers())
        )
      .subscribe()
    );
  }

  deleteCustomer(customer: CustomerModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare il cliente ${customer.name}?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._customerService.deleteCustomer(customer.id)
            .pipe(
              tap(e => this._messageBox.success(`Cliente ${customer.name} cancellato con successo`)),
              tap(() => this._readCustomers())
            )
          .subscribe()
        );
      }
    });
  }

  createAddress(customer: CustomerModel) {
    this.customerSelezionato = customer;
    const request = new AddressModel();
    request.contactId = customer.id;
    this._subscriptions.push(
        this.addressModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._addressesService.createAddress(request)),
                tap(e => this._messageBox.success(`Indirizzo creato con successo`)),
                tap(() => this._readCustomers())
            )
            .subscribe()
    );
  }

  editAddress(address: AddressModel, customer: CustomerModel) {
    this.customerSelezionato = customer;
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
            tap(() => this._readCustomers())
        )
      .subscribe()
    );
  }

  deleteAddress(address: AddressModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare l\'indirizzo?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._addressesService.deleteAddress(address.id)
            .pipe(
              tap(e => this._messageBox.success(`L\'indirizzo cancellato con successo`)),
              tap(() => this._readCustomers())
            )
          .subscribe()
        );
      }
    });
  }

  setAddressAsMain(address: AddressModel) {
    this._messageBox.confirm(`Sei sicuro di voler selezionare l\'indirizzo come principale?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._addressesService.setAddressAsMain(address.id)
            .pipe(
              tap(e => this._messageBox.success(`L\'indirizzo selezionato come principale con successo`)),
              tap(() => this._readCustomers())
            )
          .subscribe()
        );
      }
    });
  }
}
