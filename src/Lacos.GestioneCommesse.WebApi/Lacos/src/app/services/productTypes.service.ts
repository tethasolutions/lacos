import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { ProductTypeModel } from '../shared/models/product-type.model';

@Injectable()
export class ProductTypesService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/productTypes`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readProductTypes(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/productTypes?${params}`)
            .pipe(
                map(e =>
                    {
                        const productTypes: Array<ProductTypeModel> = [];
                        e.data.forEach(item => {
                            const productType: ProductTypeModel = Object.assign(new ProductTypeModel(), item);
                            productTypes.push(productType);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(productTypes) : productTypes,
                            total: e.total
                        };
                    }
                )
            );
    }

    createProductType(request: ProductTypeModel) {
        return this._http.post<number>(`${this._baseUrl}/producttype`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateProductType(request: ProductTypeModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/producttype/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteProductType(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/producttype/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getProductTypeDetail(id: number) {
        return this._http.get<ProductTypeModel>(`${this._baseUrl}/producttype-detail/${id}`)
            .pipe(
                map(e => {
                    const productType = Object.assign(new ProductTypeModel(), e);
                    return productType;
                })
            );
    }

    readProductTypesList() {
        return this._http.get<Array<ProductTypeModel>>(`${this._baseUrl}/producttypes-list`)
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
}
