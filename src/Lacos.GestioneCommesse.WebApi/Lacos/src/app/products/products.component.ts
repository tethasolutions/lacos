import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { ProductModel, ProductReadModel } from '../shared/models/product.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ProductModalComponent } from '../product-modal/product-modal.component';
import { ProductQrCodeModalComponent } from '../product-qr-code-modal/product-qr-code-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})

export class ProductsComponent extends BaseComponent implements OnInit {

  @ViewChild('productModal', { static: true }) productModal: ProductModalComponent;
  @ViewChild('productQrCodeModal', { static: true }) productQrCodeModal: ProductQrCodeModalComponent;

  pathImage = `${ApiUrls.baseUrl}/attachments/`;
  products: GridDataResult;

  stateGridProducts: State = {
    skip: 0,
    take: 30,
    filter: {
      filters: [],
      logic: 'and'
    },
    group: [],
    sort: []
  };

  productSelezionato = new ProductModel();
  screenWidth: number;

  constructor(
    private readonly _productsService: ProductsService,
    private readonly _messageBox: MessageBoxService
  ) {
    super();
  }

  ngOnInit() {
    this._readProducts();
    this.updateScreenSize();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.updateScreenSize();
  }

  private updateScreenSize(): void {
    this.screenWidth = window.innerWidth - 44;
    if (this.screenWidth > 1876) this.screenWidth = 1876;
    if (this.screenWidth < 1400) this.screenWidth = 1400;
  }


  dataStateChange(state: State) {
    this.stateGridProducts = state;
    this._readProducts();
  }

  protected _readProducts() {
    this._subscriptions.push(
      this._productsService.readProducts(this.stateGridProducts)
        .pipe(
          tap(e => {
            this.products = e;
          })
        )
        .subscribe()
    );
  }

  createProduct() {
    this.productModal.loadData();
    const request = new ProductModel();
    this._subscriptions.push(
      this.productModal.open(request)
        .pipe(
          filter(e => e),
          switchMap(() => this._productsService.createProduct(request)),
          tap(e => {
            this._messageBox.success(`Prodotto creato`);
          }),
          tap(() => this._readProducts())
        )
        .subscribe()
    );
  }

  editProduct(id: number) {
    this._subscriptions.push(
      this._productsService.getProductDetail(id)
        .pipe(
          map(e => {
            return Object.assign(new ProductModel(), e);
          }),
          switchMap(e => this.productModal.open(e)),
          filter(e => e),
          map(() => this.productModal.options),
          switchMap(e => this._productsService.updateProduct(e, id)),
          map(() => this.productModal.options),
          tap(e => this._messageBox.success(`Prodotto aggiornato`)),
          tap(() => this._readProducts())
        )
        .subscribe()
    );
  }

  deleteProduct(product: ProductModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare prodotto "${product.description}"?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._productsService.deleteProduct(product.id)
            .pipe(
              tap(e => this._messageBox.success(`Prodotto "${product.description}" cancellato con successo`)),
              tap(() => this._readProducts())
            )
            .subscribe()
        );
      }
    });
  }

  viewQrCodeProduct(product: ProductReadModel) {
    this._subscriptions.push(
      this.productQrCodeModal.open(product).subscribe()
    );
  }

  exportToExcel(): void {
    var tempState = Object.assign({}, this.stateGridProducts);
    tempState.take = this.products.total;

    this._subscriptions.push(
      this._productsService.readProducts(tempState)
        .pipe(
          tap(e => {
            this.products = e;
            const options = this.getExportOptions();
            const workbook = new Workbook(options);
            workbook.toDataURL().then((dataURL) => {
              saveAs(dataURL, 'prodotti.xlsx');
            });
            this._readProducts();
            this._messageBox.success('Esportazione avvenuta con successo');
          }),
        )
        .subscribe()
    );

  }

  private getExportOptions(): any {
    return {
      sheets: [{
        columns: [
          { autoWidth: true },
          { autoWidth: true },
          { autoWidth: true },
          { autoWidth: true },
          { autoWidth: true },
          { autoWidth: true },
          { autoWidth: true },
          { autoWidth: true }
        ],
        title: 'Prodotti',
        rows: [
          {
            cells: [
              { value: 'Tipologia Impianto', bold: true },
              { value: 'Codice', bold: true },
              { value: 'Nome', bold: true },
              { value: 'Descrizione', bold: true },
              { value: 'Matricola', bold: true },
              { value: 'Dismesso', bold: true },
              { value: 'Man. Mensile', bold: true }
            ]
          },
          ...this.products.data.map((item: any) => ({
            cells: [
              { value: item.productType },
              { value: item.code },
              { value: item.name },
              { value: item.description },
              { value: item.serialNumber },
              { value: item.isDecommissioned ? 'Sì' : 'No' },
              { value: item.monthlyMaintenance ? 'Sì' : 'No' }
            ]
          }))
        ]
      }]
    };
  }
}
