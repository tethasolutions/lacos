import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { ProductModel } from '../shared/models/product.model';
import { ProductTypeModel } from "../shared/models/product-type.model";

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
                    <GridDataResult>{
                        data: hasGroups ? translateDataSourceResultGroups(e.data) : e.data,
                        total: e.total
                    }
                )
            );
    }

    readProductTypes() {
        return this._http.get<Array<ProductTypeModel>>(`${this._baseUrl}/product-types`)
            .pipe(
                map(e =>
                    {
                        const productTypes: Array<ProductTypeModel> = [];
                        e.forEach(item => {
                            const productType: ProductTypeModel = Object.assign(new ProductTypeModel(), item);
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
                    product.productType = Object.assign(new ProductTypeModel(), product.productType);
                    return product;
                })
            );
    }

    createProduct(request: ProductModel) {
        // const formData: FormData = new FormData();
        // if (request.documents.length > 0) {
        //     formData.append('fileKey', request.documents[0], request.documents[0].originalFileName);
        // }
        return this._http.post<number>(`${this._baseUrl}/product`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateProduct(request: ProductModel, id: number) {
        // const formData: FormData = new FormData();
        // if (request.documents.length > 0) {
        //     formData.append('fileKey', request.documents[0], request.documents[0].originalFileName);
        // }
        return this._http.put<void>(`${this._baseUrl}/product/${id}`, request)
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
        const options  = {
            headers: new HttpHeaders({
              'Accept': 'text/html, application/xhtml+xml, */*',
              'Content-Type': 'text/plain; charset=utf-8'
            }),
            responseType: 'text' as 'text'
        }
        return this._http.post(`${this._baseUrl}/product-qr-code/${productId}`, null, options)
            .pipe(
                map(e => {
                    console.log(e);
                    return e;
                })
            );
    }
}
