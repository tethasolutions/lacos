import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { OperatorModel } from '../shared/models/operator.model';
import { VehicleModel } from '../shared/models/vehicle.model';
import { OperatorDocumentModel } from '../shared/models/operator-document.model';
import { ProductModel } from '../shared/models/product.model';
import { ActivityProductTypeModel } from "../shared/models/activity-product-type.model";

@Injectable()
export class ProductsService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/products`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readProducts(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/products?${params}`)
            .pipe(
                map(e =>
                    {
                        const products: Array<ProductModel> = [];
                        e.data.forEach(item => {
                            const product: ProductModel = Object.assign(new ProductModel(), item);
                            product.productType = Object.assign(new ActivityProductTypeModel(), product.productType);
                            products.push(product);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(products) : products,
                            total: e.total
                        };
                    }
                )
            );
    }

    readProductTypes() {
        return this._http.get<Array<ActivityProductTypeModel>>(`${this._baseUrl}/product-types`)
            .pipe(
                map(e =>
                    {
                        const productTypes: Array<ActivityProductTypeModel> = [];
                        e.forEach(item => {
                            const productType: ActivityProductTypeModel = Object.assign(new ActivityProductTypeModel(), item);
                            productTypes.push(productType);
                        });
                        return productTypes;
                    }
                )
            );
    }

    getProductDetail(id: number) {
        return this._http.get<ProductModel>(`${this._baseUrl}/product-detail/${id}`)
            .pipe(
                map(e => {
                    const product: ProductModel = Object.assign(new ProductModel(), e);
                    product.productType = Object.assign(new ActivityProductTypeModel(), product.productType);
                    return product;
                })
            );
    }

    createProduct(request: ProductModel) {
        const formData: FormData = new FormData();
        if (request.files.length > 0) {
            formData.append('fileKey', request.files[0], request.files[0].name);
        }
        return this._http.post<number>(`${this._baseUrl}/product`, formData)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateProduct(request: ProductModel, id: number) {
        const formData: FormData = new FormData();
        if (request.files.length > 0) {
            formData.append('fileKey', request.files[0], request.files[0].name);
        }
        return this._http.put<void>(`${this._baseUrl}/product/${id}`, formData)
            .pipe(
                map(() => { })
            );
    }

    deleteProduct(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/product/${id}`)
            .pipe(
                map(() => { })
            );
    }

    createProductQrCode(productId: number) {
        return this._http.post<string>(`${this._baseUrl}/product/${productId}`, null)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }
}
