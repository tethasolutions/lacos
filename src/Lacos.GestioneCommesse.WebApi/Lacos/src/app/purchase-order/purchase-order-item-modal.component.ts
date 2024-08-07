import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { tap } from 'rxjs';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { ProductReadModel } from '../shared/models/product.model';
import { State } from '@progress/kendo-data-query';
import { ApiUrls } from '../services/common/api-urls';
import { PurchaseOrderItem } from '../services/purchase-orders/models';

@Component({
    selector: 'app-purchase-order-item-modal',
    templateUrl: 'purchase-order-item-modal.component.html'
})
export class PurchaseOrderItemModalComponent extends ModalFormComponent<PurchaseOrderItem> implements OnInit {

    products: SelectableProduct[];

    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;

    constructor(
        messageBox: MessageBoxService,
        private readonly _productsService: ProductsService
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._readSpareParts();
    }

    onProductChange() {
        const product = this.products
            .find(e => e.id === this.options.productId);

        this.options.productName = product?.name;
        this.options.productImage = product?.pictureFileName;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _readSpareParts() {
        const state: State = {
            sort: [{ field: 'name', dir: 'asc' }]
        };

        this._subscriptions.push(
            this._productsService.readSpareParts(state)
                .pipe(
                    tap(e => this.products = (e.data as ProductReadModel[]).map(ee => new SelectableProduct(ee)))
                )
                .subscribe()
        );
    }

}

class SelectableProduct {

    readonly id: number;
    readonly code: string;
    readonly name: string;
    readonly fullName: string;
    readonly pictureFileName: string;
    readonly description: string;

    constructor(
        product: ProductReadModel
    ) {
        this.id = product.id;
        this.code = product.code;
        this.name = product.name;
        this.fullName = `${product.code} - ${product.name}`;
        this.pictureFileName = product.pictureFileName;
        this.description = product.description;
    }

}
