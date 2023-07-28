import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { ProductModel } from '../shared/models/product.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})

export class ProductsComponent extends BaseComponent implements OnInit {

  products: GridDataResult;

  stateGridProducts: State = {
      skip: 0,
      take: 10,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  productSelezionato = new ProductModel();

  constructor(
      private readonly _productsService: ProductsService,
      private readonly _messageBox: MessageBoxService
  ) {
      super();
  }

  ngOnInit() {
    this._readProducts();
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

  }

  editProduct(product: ProductModel) {

  }

  deleteProduct(product: ProductModel) {

  }

  viewQrCodeProduct(product: ProductModel) {

  }
}
