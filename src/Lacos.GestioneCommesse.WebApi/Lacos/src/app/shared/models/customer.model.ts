import { AddressModel } from './address.model';
import { CustomerFiscalTypeEnum } from '../enums/customer-fiscal-type.enum';

export class CustomerModel {
    id: number;
    name: string;
    fiscalType: CustomerFiscalTypeEnum;
    canGenerateTickets: boolean;
    telephone: string;
    email: string;
    addresses: AddressModel[];

    mainAddress: AddressModel;

    get fiscalTypeDescription(): string {
        if (this.fiscalType >= 0) { return CustomerFiscalTypeEnum[this.fiscalType]; }
        else { return ''; }
    }

    constructor() {
        this.id = null;
        this.name = null;
        this.fiscalType = null;
        this.canGenerateTickets = false;
        this.telephone = null;
        this.email = null;
        this.addresses = [];

        this.mainAddress = null;
    }
}
