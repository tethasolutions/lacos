import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { AddressModel } from '../shared/models/address.model';
import { SupplierModel } from '../shared/models/supplier.model';

@Injectable()
export class SupplierService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/suppliers`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readSuppliers(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/suppliers?${params}`)
            .pipe(
                map(e =>
                    {
                        const suppliers: Array<SupplierModel> = [];
                        e.data.forEach(item => {
                            const supplier: SupplierModel = Object.assign(new SupplierModel(), item);

                            const addresses: Array<AddressModel> = [];
                            supplier.addresses.forEach(addressitem => {
                                const address: AddressModel = Object.assign(new AddressModel(), addressitem);
                                addresses.push(address);
                            });
                            supplier.addresses = addresses;

                            let mainAddress = addresses.find(x => x.isMainAddress);
                            if (mainAddress == undefined) { mainAddress = new AddressModel(); }
                            supplier.mainAddress = mainAddress;
                            suppliers.push(supplier);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(suppliers) : suppliers,
                            total: e.total
                        };
                    }
                )
            );
    }

    createSupplier(request: SupplierModel) {
        this.setSupplierTelephoneAndEmailInMainAddress(request);
        return this._http.post<SupplierModel>(`${this._baseUrl}/supplier`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateSupplier(request: SupplierModel, id: number) {
        this.setSupplierTelephoneAndEmailInMainAddress(request);
        return this._http.put<void>(`${this._baseUrl}/supplier/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    setSupplierTelephoneAndEmailInMainAddress(request: SupplierModel) {
        const mainAddress = request.addresses.find(x => x.isMainAddress == true);
        if (mainAddress != undefined) {
            mainAddress.telephone = request.telephone;
            mainAddress.email = request.email;
        }
    }

    deleteSupplier(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/supplier/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getSupplier(id: number) {
        return this._http.get<SupplierModel>(`${this._baseUrl}/supplier/${id}`)
            .pipe(
                map(e => {
                    const supplier: SupplierModel = Object.assign(new SupplierModel(), e);

                    const addresses: Array<AddressModel> = [];
                    supplier.addresses.forEach(item => {
                        const address: AddressModel = Object.assign(new AddressModel(), item);
                        addresses.push(address);
                    });
                    supplier.addresses = addresses;

                    let mainAddress = addresses.find(x => x.isMainAddress);
                    if (mainAddress == undefined) { mainAddress = new AddressModel(); }
                    supplier.mainAddress = mainAddress;

                    supplier.telephone = mainAddress.telephone;
                    supplier.email = mainAddress.email;

                    return supplier;
                })
            );
    }

    getSuppliersList() {
        return this._http.get<Array<SupplierModel>>(`${this._baseUrl}/suppliers-list`)
            .pipe(
                map(e =>
                    {
                        const suppliers: Array<SupplierModel> = [];
                        e.forEach(item => {
                            const supplier: SupplierModel = Object.assign(new SupplierModel(), item);

                            const addresses: Array<AddressModel> = [];
                            supplier.addresses.forEach(addressitem => {
                                const address: AddressModel = Object.assign(new AddressModel(), addressitem);
                                addresses.push(address);
                            });
                            supplier.addresses = addresses;

                            let mainAddress = addresses.find(x => x.isMainAddress);
                            if (mainAddress == undefined) { mainAddress = new AddressModel(); }
                            supplier.mainAddress = mainAddress;
                            suppliers.push(supplier);
                        });
                        return suppliers;
                    }
                )
            );
    }
}
