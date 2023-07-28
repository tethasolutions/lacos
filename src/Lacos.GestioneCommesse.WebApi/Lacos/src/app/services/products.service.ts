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
}
