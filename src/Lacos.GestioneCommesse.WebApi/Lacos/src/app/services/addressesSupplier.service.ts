import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { AddressSupplierModel } from '../shared/models/address-supplier.model';

@Injectable()
export class AddressesSupplierService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/addresses`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    createAddress(request: AddressSupplierModel) {
        return this._http.post<number>(`${this._baseUrl}/address`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateAddress(request: AddressSupplierModel, id: number) {
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
        return this._http.get<AddressSupplierModel>(`${this._baseUrl}/address/${id}`)
            .pipe(
                map(e => {
                    const address = Object.assign(new AddressSupplierModel(), e);
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
        return this._http.get<Array<AddressSupplierModel>>(`${this._baseUrl}/address/customer/${customerId}`)
            .pipe(
                map(result => {
                    const addresses: Array<AddressSupplierModel> = [];
                    result.forEach(item => {
                        const address = Object.assign(new AddressSupplierModel(), item);
                        addresses.push(address);
                    });
                    return addresses;
                })
            );
    }
}
