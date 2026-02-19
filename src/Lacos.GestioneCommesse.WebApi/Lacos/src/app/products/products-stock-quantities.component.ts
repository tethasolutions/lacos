import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { ProductModel } from '../shared/models/product.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ProductModalComponent } from '../product-modal/product-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';
import { WarehouseMovementsModalComponent } from '../warehouse-movements/warehouse-movements-modal.component';

@Component({
  selector: 'app-products-stock-quantities',
  templateUrl: './products-stock-quantities.component.html'
})

export class ProductsStockQuantitiesComponent extends BaseComponent implements OnInit {

  @ViewChild('productModal', { static: true }) productModal: ProductModalComponent;
  @ViewChild('warehouseMovementsModal', { static: true }) warehouseMovementsModal: WarehouseMovementsModalComponent;

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

  screenWidth: number;

  constructor(
    private readonly _productsService: ProductsService,
    private readonly _messageBox: MessageBoxService
  ) {
    super();
  }

  ngOnInit() {
    this._readWarehouseProducts();
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
    this._readWarehouseProducts();
  }

  protected _readWarehouseProducts() {
    this._subscriptions.push(
      this._productsService.readProductsStockQuantities(this.stateGridProducts)
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
          tap(() => this._readWarehouseProducts())
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
          tap(() => this._readWarehouseProducts())
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
              tap(() => this._readWarehouseProducts())
            )
            .subscribe()
        );
      }
    });
  }

  viewMovements(product: any) {
    this._subscriptions.push(
      this.warehouseMovementsModal.open(product.id, product.name)
        .pipe(
          tap(() => this._readWarehouseProducts())
        )
        .subscribe()
    );
  }

  exportToExcel(): void {
    var tempState = Object.assign({}, this.stateGridProducts);
    tempState.take = this.products.total;

    this._subscriptions.push(
      this._productsService.readProductsStockQuantities(tempState)
        .pipe(
          tap(e => {
            this.products = e;
            const options = this.getExportOptions();
            const workbook = new Workbook(options);
            workbook.toDataURL().then((dataURL) => {
              saveAs(dataURL, 'magazzino.xlsx');
            });
            this._readWarehouseProducts();
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
          { autoWidth: true },
          { autoWidth: true }
        ],
        title: 'Magazzino - Giacenza Prodotti',
        rows: [
          {
            cells: [
              { value: 'Tipologia', bold: true },
              { value: 'Codice', bold: true },
              { value: 'Nome', bold: true },
              { value: 'Descrizione', bold: true },
              { value: 'Marca', bold: true },
              { value: 'Lato', bold: true },
              { value: 'Dimensione', bold: true },
              { value: 'Materiale', bold: true },
              { value: 'Giacenza', bold: true }
            ]
          },
          ...this.products.data.map((item: any) => ({
            cells: [
              { value: item.productType },
              { value: item.code },
              { value: item.name },
              { value: item.description },
              { value: item.brand },
              { value: item.side },
              { value: item.size },
              { value: item.material },
              { value: item.stockQuantity }
            ]
          }))
        ]
      }]
    };
  }
}
