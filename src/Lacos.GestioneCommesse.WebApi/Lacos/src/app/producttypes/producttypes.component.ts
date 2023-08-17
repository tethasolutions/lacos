import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, 
  GridComponent,
  CancelEvent,
  EditEvent,
  RemoveEvent,
  SaveEvent,
  AddEvent, } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { ProductTypesService } from '../services/productTypes.service';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { ProductTypeModalComponent } from '../producttype-modal/producttype-modal.component';

@Component({
  selector: 'app-producttypes',
  templateUrl: './producttypes.component.html',
  styleUrls: ['./producttypes.component.scss']
})

export class ProductTypesComponent extends BaseComponent implements OnInit {
  dataProductTypes: GridDataResult;
  stateGridProductTypes: State = {
      skip: 0,
      take: 10,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };
  
  @Input() productType = new ProductTypeModel();
  @ViewChild('productTypeModal', { static: true }) productTypeModal: ProductTypeModalComponent;

  constructor(
      private readonly _productTypesService: ProductTypesService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readProductTypes();
  }

  dataStateChange(state: State) {
      this.stateGridProductTypes = state;
      this._readProductTypes();
  }

  protected _readProductTypes() {
    this._subscriptions.push(
      this._productTypesService.readProductTypes(this.stateGridProductTypes)
        .pipe(
            tap(e => {
              this.dataProductTypes = e;
            })
        )
        .subscribe()
    );
  }

  createProductType() {
    const request = new ProductTypeModel();
    this._subscriptions.push(
        this.productTypeModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._productTypesService.createProductType(request)),
                tap(e => {
                  this._messageBox.success(`Tipo ${request.name} creato`);
                }),
                tap(() => this._readProductTypes())
            )
            .subscribe()
    );
  }

  editProductType(productType: ProductTypeModel) {
    this._subscriptions.push(
      this._productTypesService.getProductTypeDetail(productType.id)
        .pipe(
            map(e => {
              return Object.assign(new ProductTypeModel(), e);
            }),
            switchMap(e => this.productTypeModal.open(e)),
            filter(e => e),
            map(() => this.productTypeModal.options),
            switchMap(e => this._productTypesService.updateProductType(e, e.id)),
            map(() => this.productTypeModal.options),
            tap(e => this._messageBox.success(`Tipo ${productType.name} aggiornato`)),
            tap(() => this._readProductTypes())
        )
      .subscribe()
    );
  }

  deleteProductType(productType: ProductTypeModel) {
    this._messageBox.info('Eliminazione elemento non attiva');
    // this._messageBox.confirm(`Sei sicuro di voler cancellare il tipo ${productType.name}?`, 'Conferma l\'azione').subscribe(result => {
    //   if (result == true) {
    //     this._subscriptions.push(
    //       this._productTypesService.deleteProductType(productType.id)
    //         .pipe(
    //           tap(e => this._messageBox.success(`Tipo ${productType.name} cancellato con successo`)),
    //           tap(() => this._readProductTypes())
    //         )
    //       .subscribe()
    //     );
    //   }
    // });
  }

}
