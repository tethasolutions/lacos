import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { tap } from 'rxjs';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { ProductReadModel } from '../shared/models/product.model';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { State } from '@progress/kendo-data-query';
import { ApiUrls } from '../services/common/api-urls';
import { PurchaseOrderItem } from '../services/purchase-orders/models';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';

@Component({
    selector: 'app-purchase-order-item-modal',
    templateUrl: 'purchase-order-item-modal.component.html'
})
export class PurchaseOrderItemModalComponent extends ModalFormComponent<PurchaseOrderItem> implements OnInit {

    private _allProducts: SelectableProduct[] = [];

    productTypes: ProductTypeModel[] = [];
    selectedProductTypeId: number | null = null;
    products: SelectableProduct[] = [];

    readonly isAdmin: boolean = false;
    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;

    constructor(
        messageBox: MessageBoxService,
        private readonly security: SecurityService,
        private readonly _productsService: ProductsService
    ) {
        super(messageBox);
        this.isAdmin = security.isAuthorized(Role.Administrator);
    }

    ngOnInit() {
        this._readProductTypes();
        this._readSpareParts();
    }

    override open(item: PurchaseOrderItem) {
        const result = super.open(item);
        this.selectedProductTypeId = null;
        this.products = [];

        if (item.productId) {
            this._syncProductTypeFromProduct(item.productId);
        }

        return result;
    }

    onProductTypeChange() {
        this.options.productId = null;
        this.options.productName = null;
        this.options.productImage = null;
        this._filterProducts();
    }

    onProductChange() {
        const product = this.products
            .find(e => e.id === this.options.productId);

        this.options.productName = product?.name;
        this.options.productImage = product?.pictureFileName;
        this.options.unitPrice = product?.defaultPrice;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        if (this.form.valid) {
            this.options.totalAmount = (this.options.quantity ?? 0) * (this.options.unitPrice ?? 0);
        }

        return this.form.valid;
    }

    private _filterProducts() {
        if (this.selectedProductTypeId == null) {
            this.products = [];
        } else {
            this.products = this._allProducts.filter(p => p.productTypeId === this.selectedProductTypeId);
        }
    }

    private _syncProductTypeFromProduct(productId: number) {
        const product = this._allProducts.find(p => p.id === productId);
        if (product) {
            this.selectedProductTypeId = product.productTypeId;
            this._filterProducts();
        }
    }

    private _readProductTypes() {
        this._subscriptions.push(
            this._productsService.readProductTypes()
                .pipe(
                    tap(e => this.productTypes = e)
                )
                .subscribe()
        );
    }

    private _readSpareParts() {
        const state: State = {
            sort: [{ field: 'name', dir: 'asc' }]
        };

        this._subscriptions.push(
            this._productsService.readSpareParts(state)
                .pipe(
                    tap(e => {
                        this._allProducts = (e.data as ProductReadModel[]).map(ee => new SelectableProduct(ee));
                        if (this.options?.productId) {
                            this._syncProductTypeFromProduct(this.options.productId);
                        }
                    })
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
    readonly productTypeId: number;
    readonly defaultPrice: number;

    constructor(
        product: ProductReadModel
    ) {
        this.id = product.id;
        this.code = product.code;
        this.name = product.name;
        this.fullName = `${product.code} - ${product.name}`;
        this.pictureFileName = product.pictureFileName;
        this.description = product.description;
        this.productTypeId = product.productTypeId;
        this.defaultPrice = product.defaultPrice;
    }

}
