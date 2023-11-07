import { AddressModel } from './address.model';

export class SupplierModel {
    id: number;
    name: string;
    telephone: string;
    email: string;
    addresses: AddressModel[];

    mainAddress: AddressModel;

    constructor() {
        this.id = null;
        this.name = null;
        this.telephone = null;
        this.email = null;
        this.addresses = [];

        this.mainAddress = null;
    }
}
