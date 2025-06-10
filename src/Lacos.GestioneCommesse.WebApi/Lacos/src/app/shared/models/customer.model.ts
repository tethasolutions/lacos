import { AddressModel } from './address.model';
import { CustomerFiscalTypeEnum } from '../enums/customer-fiscal-type.enum';

export class CustomerModel {
    id: number;
    name: string;
    fiscalType: CustomerFiscalTypeEnum;
    canGenerateTickets: boolean;
    telephone: string;
    email: string;
    contact: string;
    contactTelephone: string;
    contactEmail: string;
    sdiCode: string;
    vatNumber: string;
    fiscalCode: string;
    addresses: AddressModel[];

    mainAddress: AddressModel;
    mainFullAddress: string;

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
        this.contact = null;
        this.contactTelephone = null;
        this.contactEmail = null;
        this.sdiCode = null;
        this.vatNumber = null;
        this.fiscalCode = null;
        this.addresses = [];

        this.mainAddress = null;
        this.mainFullAddress = null;
    }
}
