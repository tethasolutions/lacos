import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { tap } from 'rxjs';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { InterventionProduct } from '../services/intervention-products/models';
import { ProductTypesService } from '../services/productTypes.service';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { ProductsService } from '../services/products.service';
import { ProductReadModel } from '../shared/models/product.model';
import { State } from '@progress/kendo-data-query';
import { ApiUrls } from '../services/common/api-urls';

@Component({
    selector: 'app-intervention-product-modal',
    templateUrl: 'intervention-product-modal.component.html'
})
export class InterventionProductModalComponent extends ModalComponent<InterventionProductModalOptions> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    productTypes: SelectableProductType[];
    productType: SelectableProductType;
    products: SelectableProduct[];
    product: SelectableProduct;

    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;

    constructor(
        private readonly _productTypesService: ProductTypesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _productsService: ProductsService
    ) {
        super();
    }

    ngOnInit() {
        this._getProductTypes();
    }

    override open(options: InterventionProductModalOptions) {
        const result = super.open(options);

        this.productType = null;
        this.products = [];
        this.product = null;

        return result;
    }

    onProductTypeChanged() {
        this.product = null;
        this.options.product.productId = this.product?.id;

        if (!this.productType) {
            this.products = [];
            return;
        }

        const state: State = {
            filter: {
                filters: [
                    {
                        filters: [
                            { field: 'customerAddressId', operator: 'isnull' },
                            { field: 'customerAddressId', operator: 'eq', value: this.options.customerAddressId }
                        ],
                        logic: 'or'
                    },
                    { field: 'productTypeId', operator: 'eq', value: this.productType.id }
                ],
                logic: 'and'
            },
            sort: [{ field: 'name', dir: 'asc' }]
        };

        this._subscriptions.push(
            this._productsService.readProducts(state)
                .pipe(
                    tap(e => this.products = (e.data as ProductReadModel[]).map(ee => new SelectableProduct(ee)))
                )
                .subscribe()
        );
    }

    onProductChange() {
        this.options.product.productId = this.product?.id;
    }

    protected override _canClose() {
        markAsDirty(this.form);

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getProductTypes() {
        this._subscriptions.push(
            this._productTypesService.readProductTypesList()
                .pipe(
                    tap(e => this.productTypes = e.map(ee => new SelectableProductType(ee)))
                )
                .subscribe()
        );
    }

}

export class InterventionProductModalOptions {

    constructor(
        readonly customerAddressId: number,
        readonly product: InterventionProduct
    ) {
    }

}

class SelectableProductType {

    readonly id: number;
    readonly code: string;
    readonly name: string;
    readonly fullName: string;

    constructor(
        productType: ProductTypeModel
    ) {
        this.id = productType.id;
        this.code = productType.code;
        this.name = productType.name;
        this.fullName = `${productType.code} - ${productType.name}`;
    }

}

class SelectableProduct {

    readonly id: number;
    readonly code: string;
    readonly name: string;
    readonly fullName: string;
    readonly pictureFileName: string;

    constructor(
        product: ProductReadModel
    ) {
        this.id = product.id;
        this.code = product.code;
        this.name = product.name;
        this.fullName = `${product.code} - ${product.name}`;
        this.pictureFileName = product.pictureFileName;
    }

}
