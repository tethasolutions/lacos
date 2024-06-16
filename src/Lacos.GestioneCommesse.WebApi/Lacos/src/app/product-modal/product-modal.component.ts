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
import { ApiUrls } from '../services/common/api-urls';
import { RemoveEvent, SuccessEvent, FileInfo,FileState,SelectEvent} from "@progress/kendo-angular-upload";
import { UploadFileModel } from '../shared/models/upload-file.model';
import { ProductDocumentModel } from '../shared/models/product-document.model';

@Component({
  selector: 'app-product-modal',
  templateUrl: './product-modal.component.html',
  styleUrls: ['./product-modal.component.scss']
})

export class ProductModalComponent extends ModalComponent<ProductModel> {

  @ViewChild('form') form: NgForm;
  @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
  @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
  
  private readonly _baseUrl = `${ApiUrls.baseApiUrl}/products`;
  uploadSaveUrl = `${this._baseUrl}/document/upload-file`;
  uploadRemoveUrl = `${this._baseUrl}/document/remove-file`; 
  productTypes: Array<ProductTypeModel> = [];
  customers: Array<CustomerModel> = [];
  customerSelezionato = new CustomerModel();

  documents: Array<FileInfo> = [];
  attachmentsUploads: Array<UploadFileModel> =[];
  isUploaded:Array<boolean>= [];
  get qrCode(){
    return (this.options.qrCodePrefix ?? '') + (this.options.qrCodeNumber ?? "");
  }

  pathImage = `${ApiUrls.baseUrl}/attachments/`;
  attachmentsFileInfo:any;
  isImpiantoPortaRei = false;
  imageLabel: string;

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
    this.options.addressId = null;
    if (customerId == undefined) { 
      this.customerSelezionato = new CustomerModel();
      return; 
    }
    const nuovoCustomerSelezionato: CustomerModel = this.customers.find(x => x.id === customerId);
    if (nuovoCustomerSelezionato == undefined) { return; }
    this.customerSelezionato = nuovoCustomerSelezionato;
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
                  this.options.customerId = e.id;
                  this._messageBox.success(`Cliente ${request.name} creato`);
                }),
                tap(() => this._readCustomers(true))
            )
            .subscribe()
    );
  }

  createAddress() {
    const request = new AddressModel();
    request.customerId = this.options.customerId;
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
                this.options.addressId = e.id;
                address.id = e.id;
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

  public ImageExecutionSuccess(e: SuccessEvent): void
  {
    const body = e.response.body;
    if(body != null)
    {

      const uploadedFile = body as UploadFileModel;
      const operatorAttachment = new UploadFileModel(uploadedFile.fileName,uploadedFile.originalFileName);
      this.options.pictureFileName = uploadedFile.fileName;   
      this.isUploaded.push(true);
    }
    else
    {
      const deletedFile = e.files[0].name;
      const index = this.attachmentsUploads.findIndex(x=>x.originalFileName == deletedFile);
      if(index>-1)
      {
      this.attachmentsUploads.splice(index,1);
      this.isUploaded.pop();
      }
    }
  }

  downloadAttachment(fileName: string) {
    const attachment = this.options.documents
        .find(e => e.originalFileName === fileName);
    const url = `${this._baseUrl}/product-document/download-file/${attachment.fileName}/${attachment.originalFileName}`;

    window.open(url);
}

  public DocumentExecutionSuccess(e: SuccessEvent): void
  {const file = e.response.body as ProductDocumentModel;
    if (file != null) {
        let productDocumentModel = new ProductDocumentModel(0, this.options.id, file.originalFileName, file.fileName, this.options.name);
        this.options.documents.push(productDocumentModel);
    } else {
        const deletedFile = e.files[0].name;
        this.options.documents.findAndRemove(e => e.originalFileName === deletedFile);
    }
  }

  // public AttachmentSelect(e: SelectEvent): void
  // {
  //   const files = e.files;
  //   let popup = false;
  //   files.forEach(element => {
  //     var index = this.attachmentsUploads.findIndex(x=>x.originalFileName == element.name);
  //     if(index > -1)
  //     {
  //       files.splice(index,1);
  //     popup = true;
  //     }
  //   });     
  //   if(popup)
  //   {
  //     this._messageBox.alert(`Sono presenti tra i file caricati alcuni file con lo stesso nome di quelli che si vogliono caricare`);
  //   }
  // }

  public loadData() {
    this.attachmentsFileInfo = null;
    if (this.options.pictureFileName != null) 
      this.imageLabel = "Cambia immagine"
    else
      this.imageLabel = "Immagine"
    
    this.documents = [];
    if (this.options.documents != null) {
        this.options.documents.forEach(element => {
            if (element.originalFileName != null && element.fileName != null) {
                this.documents.push({ name: element.originalFileName });
            }
        });
    }

    this._readProductTypes();
    this._readCustomers();
  }
}
