import { AddressSupplierModel } from './address-supplier.model';

export class SupplierModel {
    id: number;
    name: string;
    telephone: string;
    email: string;
    addresses: AddressSupplierModel[];

    mainAddress: AddressSupplierModel;

    constructor() {
        this.id = null;
        this.name = null;
        this.telephone = null;
        this.email = null;
        this.addresses = [];

        this.mainAddress = null;
    }
}
