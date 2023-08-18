import { Component, ViewChild, Input } from '@angular/core';
import { ProductModel } from '../shared/models/product.model';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { CustomerService } from '../services/customer.service';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { CustomerModel } from '../shared/models/customer.model';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { AddressModel } from '../shared/models/address.model';
import { AddressesService } from '../services/addresses.service';

@Component({
  selector: 'app-product-modal',
  templateUrl: './product-modal.component.html',
  styleUrls: ['./product-modal.component.scss']
})

export class ProductModalComponent extends ModalComponent<ProductModel> {

  @ViewChild('form') form: NgForm;
  @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
  @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
  productTypes: Array<ProductTypeModel> = [];
  customers: Array<CustomerModel> = [];
  customerSelezionato = new CustomerModel();

  isImpiantoPortaRei = false;

  constructor(
      private readonly _messageBox: MessageBoxService,
      private readonly _productsService: ProductsService,
      private readonly _customerService: CustomerService,
      private readonly _addressesService: AddressesService
  ) {
      super();
      this.openedEvent.subscribe(item => {
        this.loadData();
      });
  }

  protected _canClose() {
    markAsDirty(this.form);

    if (this.form.invalid) {
        this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid;
  }

  protected _readProductTypes() {
    this._subscriptions.push(
      this._productsService.readProductTypes()
        .pipe(
            tap(e => {
              this.productTypes = e;
              this.checkIfImpiantoIsPortaRei();
            })
        )
        .subscribe()
    );
  }

  protected _readCustomers(creatoNuovoCustomer = false) {
    this._subscriptions.push(
      this._customerService.getCustomersList()
        .pipe(
            tap(e => {
              this.customers = e;
              // console.log(this.options);
              const customerSelezionato: CustomerModel = this.customers.find(x => x.id === this.options.customerId);
              if (customerSelezionato != undefined) { this.customerSelezionato = customerSelezionato; }
              if (creatoNuovoCustomer) {
                this.customerChanged(this.options.customerId);
              }
            })
        )
        .subscribe()
    );
  }

  customerChanged(customerId: number) {
    this.options.customerAddressId = null;
    if (customerId == undefined) { 
      this.customerSelezionato = new CustomerModel();
      return; 
    }
    const nuovoCustomerSelezionato: CustomerModel = this.customers.find(x => x.id === customerId);
    if (nuovoCustomerSelezionato == undefined) { return; }
    this.customerSelezionato = nuovoCustomerSelezionato;
  }

  protected generaQrCode() {
    this._subscriptions.push(
      this._productsService.createProductQrCode(this.options.id)
        .pipe(
            tap(e => {
              this.options.qrCode = e;
              this._messageBox.success(`QR Code generato con successo`);
              console.log(e);
            })
        )
        .subscribe()
    );
  }

  createCustomer() {
    const request = new CustomerModel();
    request.fiscalType = 0;

    this._subscriptions.push(
        this.customerModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._customerService.createCustomer(request)),
                tap(e => {
                  this.options.customerId = e;
                  this._messageBox.success(`Cliente ${request.name} creato`);
                }),
                tap(() => this._readCustomers(true))
            )
            .subscribe()
    );
  }

  createAddress() {
    const request = new AddressModel();
    request.contactId = this.options.customerId;
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
    this._subscriptions.push(
      this._addressesService.createAddress(address)
          .pipe(
              map(e => e),
              tap(e => {
                this.options.customerAddressId = e;
                address.id = e;
                const customerSelezionato: CustomerModel = this.customers.find(x => x.id === this.options.customerId);
                if (customerSelezionato != undefined) { 
                  customerSelezionato.addresses.push(address);
                }
                this._messageBox.success(`Indirizzo creato con successo`)
              }),
              tap(() => {
                // this._readCustomers(true);
              })
          )
          .subscribe()
    );
  }

  checkIfImpiantoIsPortaRei() {
    this.isImpiantoPortaRei = false;
    if (this.options.productTypeId == null || this.options.productTypeId == undefined) { return; }
    const productTypeSelezionato: ProductTypeModel = this.productTypes.find(x => x.id === this.options.productTypeId);
    if (productTypeSelezionato != undefined) {
      this.isImpiantoPortaRei = productTypeSelezionato.isReiDoor;
    }
  }

  public loadData() {
    this._readProductTypes();
    this._readCustomers();
  }
}
