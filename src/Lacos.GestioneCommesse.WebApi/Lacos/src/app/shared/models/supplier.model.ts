import { AddressModel } from './address.model';

export class SupplierModel {
    id: number;
    name: string;
    telephone: string;
    email: string;
    contact: string;
    contactTelephone: string;
    contactEmail: string;
    addresses: AddressModel[];

    mainAddress: AddressModel;

    constructor() {
        this.id = null;
        this.name = null;
        this.telephone = null;
        this.email = null;
        this.contact = null;
        this.contactTelephone = null;
        this.contactEmail = null;
        this.addresses = [];

        this.mainAddress = null;
    }
}
