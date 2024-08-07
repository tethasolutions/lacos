import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
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
import { StorageService } from '../services/common/storage.service';

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
    take: 30,
    filter: {
      filters: [],
      logic: 'and'
    },
    group: [],
    sort: []
  };

  customerSelezionato = new CustomerModel();
  screenWidth: number;

  constructor(
    private readonly _customerService: CustomerService,
    private readonly _addressesService: AddressesService,
    private readonly _messageBox: MessageBoxService,
    private readonly _storageService: StorageService
  ) {
    super();
  }

  ngOnInit() {
    this._resumeState();
    this._readCustomers();
    this.updateScreenSize();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.updateScreenSize();
  }

  private updateScreenSize(): void {
    this.screenWidth = window.innerWidth -44;
    if (this.screenWidth > 1876) this.screenWidth = 1876;
    if (this.screenWidth < 1400) this.screenWidth = 1400;     
  }


  dataStateChange(state: State) {
    this.stateGridCustomers = state;
    this._saveState();
    this._readCustomers();
  }

  private _resumeState() {
    const savedState = this._storageService.get<State>(window.location.hash, true);
    if (savedState == null) return;
    this.stateGridCustomers = savedState;
  }

  private _saveState() {
    this._storageService.save(this.stateGridCustomers, window.location.hash, true);
  }

  protected _readCustomers() {
    this._subscriptions.push(
      this._customerService.readCustomers(this.stateGridCustomers)
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
            this._messageBox.success(`Cliente ${request.name} creato`);
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
    request.customerId = customer.id;
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
