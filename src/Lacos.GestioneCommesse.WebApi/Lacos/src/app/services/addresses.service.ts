import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { AddressModel } from '../shared/models/address.model';

@Injectable()
export class AddressesService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/addresses`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    createAddress(request: AddressModel) {
        return this._http.post<number>(`${this._baseUrl}/address`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateAddress(request: AddressModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/address/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteAddress(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/address/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getAddress(id: number) {
        return this._http.get<AddressModel>(`${this._baseUrl}/address/${id}`)
            .pipe(
                map(e => {
                    const address = Object.assign(new AddressModel(), e);
                    return address;
                })
            );
    }

    setAddressAsMain(id: number) {
        return this._http.put<void>(`${this._baseUrl}/set-address-as-main/${id}`, null)
            .pipe(
                map(() => { })
            );
    }
    
    getCustomerAddresses(customerId: number) {      
        return this._http.get<Array<AddressModel>>(`${this._baseUrl}/address/customer/${customerId}`)
            .pipe(
                map(result => {
                    const addresses: Array<AddressModel> = [];
                    result.forEach(item => {
                        const address = Object.assign(new AddressModel(), item);
                        addresses.push(address);
                    });
                    return addresses;
                })
            );
    }
    
    getSupplierAddresses(supplierId: number) {      
        return this._http.get<Array<AddressModel>>(`${this._baseUrl}/address/supplier/${supplierId}`)
            .pipe(
                map(result => {
                    const addresses: Array<AddressModel> = [];
                    result.forEach(item => {
                        const address = Object.assign(new AddressModel(), item);
                        addresses.push(address);
                    });
                    return addresses;
                })
            );
    }
}
